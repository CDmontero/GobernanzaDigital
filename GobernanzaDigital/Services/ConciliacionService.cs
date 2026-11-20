using GobernanzaDigital.Data;
using GobernanzaDigital.DTOs;
using GobernanzaDigital.Exceptions;
using GobernanzaDigital.Interfaces;
using GobernanzaDigital.Models;
using Microsoft.EntityFrameworkCore;

namespace GobernanzaDigital.Services
{
    public class ConciliacionService : IConciliacionService
    {
        // Si la pasarela falla 3 veces seguidas, cortamos la conciliación
        private const int MaximoErroresSeguidos = 3;

        private readonly GobernanzaContext _context;
        private readonly IPasarelaClient _cliente;
        private readonly ILogger<ConciliacionService> _logger;

        public ConciliacionService(GobernanzaContext context, IPasarelaClient cliente, ILogger<ConciliacionService> logger)
        {
            _context = context;
            _cliente = cliente;
            _logger = logger;
        }

        public async Task<ConciliacionResultadoDto?> ConciliarAsync(ConciliacionCreateDto dto)
        {
            var pasarela = await _context.Pasarelas.FindAsync(dto.PasarelaId);
            if (pasarela == null)
            {
                return null;
            }

            _logger.LogInformation("Iniciando la conciliación con la pasarela {PasarelaId}", pasarela.Id);

            var resultado = new ConciliacionResultadoDto { PasarelaId = pasarela.Id };

            // Boletas sin pagar (las vencidas también, porque se pueden pagar tarde)
            var boletas = await _context.Boletas
                .Where(b => b.Estado == EstadoBoleta.Pendiente || b.Estado == EstadoBoleta.Vencida)
                .ToListAsync();

            int erroresSeguidos = 0;
            bool abortada = false;

            foreach (var boleta in boletas)
            {
                PagoPasarelaDto? pago;

                try
                {
                    pago = await _cliente.ConsultarPagoAsync(pasarela, boleta.CodigoPago);
                    erroresSeguidos = 0;
                }
                catch (PasarelaException ex)
                {
                    resultado.ConError++;
                    erroresSeguidos++;
                    _logger.LogWarning(ex, "Falló la consulta a la pasarela para la boleta {BoletaId}", boleta.Id);

                    if (erroresSeguidos >= MaximoErroresSeguidos)
                    {
                        abortada = true;
                        break;
                    }
                    continue;
                }

                resultado.Consultadas++;

                if (pago == null)
                {
                    resultado.SinPagoRegistrado++;
                    continue;
                }

                if (string.Equals(pago.State, "APPROVED", StringComparison.OrdinalIgnoreCase))
                {
                    if (pago.Amount != boleta.Monto)
                    {
                        _logger.LogWarning("El monto pagado de la boleta {BoletaId} no coincide con el monto de la boleta", boleta.Id);
                    }

                    boleta.Estado = EstadoBoleta.Pagada;
                    boleta.FechaPago = pago.PaymentDate ?? DateTime.Now;
                    resultado.Pagadas++;
                }
                else
                {
                    resultado.PagoNoAprobado++;
                }
            }

            // Las pendientes que ya pasaron su vencimiento pasan a Vencida
            DateTime hoy = DateTime.Today;
            foreach (var boleta in boletas)
            {
                if (boleta.Estado == EstadoBoleta.Pendiente && boleta.FechaVencimiento.Date < hoy)
                {
                    boleta.Estado = EstadoBoleta.Vencida;
                    resultado.MarcadasVencidas++;
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Conciliación terminada con la pasarela {PasarelaId}: {Pagadas} pagadas, {Errores} con error, {Vencidas} marcadas vencidas",
                pasarela.Id, resultado.Pagadas, resultado.ConError, resultado.MarcadasVencidas);

            if (abortada)
            {
                // lo conciliado hasta ahora ya quedó guardado
                throw new PasarelaException(
                    "La pasarela falló " + MaximoErroresSeguidos + " veces seguidas y se cortó la conciliación. Lo conciliado hasta ahora quedó guardado.");
            }

            return resultado;
        }
    }
}
