using System.Text.RegularExpressions;
using GobernanzaDigital.Data;
using GobernanzaDigital.DTOs;
using GobernanzaDigital.Exceptions;
using GobernanzaDigital.Interfaces;
using GobernanzaDigital.Models;
using Microsoft.EntityFrameworkCore;

namespace GobernanzaDigital.Services
{
    public class GeneracionService : IGeneracionService
    {
        private readonly GobernanzaContext _context;
        private readonly ILogger<GeneracionService> _logger;

        public GeneracionService(GobernanzaContext db, ILogger<GeneracionService> logger)
        {
            _context = db;
            _logger = logger;
        }

        public async Task<GeneracionResultadoDto> GenerarBoletasAsync(GeneracionCreateDto dto)
        {
            _logger.LogInformation("Iniciando la generación de boletas para el período {Periodo}", dto.Periodo);

            // el DTO ya valida el formato, pero lo revisamos de nuevo
            if (!Regex.IsMatch(dto.Periodo, @"^\d{4}/(0[1-9]|1[0-2])$"))
            {
                throw new ReglaNegocioException("El período debe tener el formato AAAA/MM (por ejemplo 2026/01).");
            }

            // 1) Fechas del período
            int anio = int.Parse(dto.Periodo.Substring(0, 4));
            int mes = int.Parse(dto.Periodo.Substring(5, 2));
            DateTime primerDiaPeriodo = new DateTime(anio, mes, 1);
            DateTime primerDiaSiguiente = primerDiaPeriodo.AddMonths(1);

            // vence el día 10 del mes siguiente
            DateTime fechaVencimiento = new DateTime(primerDiaSiguiente.Year, primerDiaSiguiente.Month, 10);

            // 2) Asignaciones activas en el período
            var asignaciones = await _context.ContribuyenteServicios
                .Include(a => a.Servicio)
                .Where(a => a.FechaAlta < primerDiaSiguiente && (a.FechaBaja == null || a.FechaBaja >= primerDiaPeriodo))
                .ToListAsync();

            _logger.LogInformation("Se encontraron {Cantidad} asignaciones activas en el período {Periodo}",
                asignaciones.Count, dto.Periodo);

            // 3) Boletas que ya existen en este período (una sola consulta)
            var existentes = await _context.Boletas
                .Where(b => b.Periodo == dto.Periodo)
                .Select(b => new { b.ContribuyenteId, b.ServicioId, b.CodigoPago })
                .ToListAsync();

            var yaGeneradas = new HashSet<(int, int)>(existentes.Select(x => (x.ContribuyenteId, x.ServicioId)));
            var codigosUsados = new HashSet<string>(existentes.Select(x => x.CodigoPago));

            var resultado = new GeneracionResultadoDto { Periodo = dto.Periodo };

            // 4) Recorremos las asignaciones
            foreach (var asignacion in asignaciones)
            {
                var servicio = asignacion.Servicio;
                if (servicio == null || servicio.FrecuenciaEnMeses < 1)
                {
                    _logger.LogWarning("La asignación {AsignacionId} tiene un servicio o una frecuencia inválida y se saltea",
                        asignacion.Id);
                    resultado.BoletasInvalidas++;
                    continue;
                }

                // ¿Le toca boleta en este período? La frecuencia se cuenta desde la fecha de alta
                int mesesDesdeAlta = (anio - asignacion.FechaAlta.Year) * 12 + (mes - asignacion.FechaAlta.Month);
                bool leToca = mesesDesdeAlta >= 0 && mesesDesdeAlta % servicio.FrecuenciaEnMeses == 0;
                if (!leToca)
                {
                    continue;
                }

                // ¿Ya existe la boleta? (evita duplicados)
                if (yaGeneradas.Contains((asignacion.ContribuyenteId, asignacion.ServicioId)))
                {
                    resultado.BoletasOmitidas++;
                    continue;
                }

                // Monto: el personalizado si hay, si no el base del servicio
                decimal monto = asignacion.MontoPersonalizado ?? servicio.MontoBase;
                if (monto <= 0)
                {
                    _logger.LogWarning("La asignación {AsignacionId} tiene un monto inválido y se saltea", asignacion.Id);
                    resultado.BoletasInvalidas++;
                    continue;
                }

                var boleta = new Boleta
                {
                    ContribuyenteId = asignacion.ContribuyenteId,
                    ServicioId = asignacion.ServicioId,
                    Periodo = dto.Periodo,
                    FechaVencimiento = fechaVencimiento,
                    Monto = monto,
                    CodigoPago = NuevoCodigoPago(dto.Periodo, codigosUsados),
                    Estado = EstadoBoleta.Pendiente,
                    FechaPago = null
                };

                _context.Boletas.Add(boleta);
                yaGeneradas.Add((asignacion.ContribuyenteId, asignacion.ServicioId));
                resultado.BoletasCreadas++;
            }

            // 5) Un solo guardado al final
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Por si el índice único de la base frena un duplicado
                // (por ejemplo si dos personas generan el mismo período a la vez)
                _logger.LogError(ex, "No se pudieron guardar las boletas del período {Periodo}", dto.Periodo);
                throw new ReglaNegocioException(
                    "No se pudieron guardar las boletas: otro proceso ya generó boletas de este período. Probá de nuevo.", true);
            }

            _logger.LogInformation(
                "Generación terminada para el período {Periodo}: {Creadas} creadas, {Omitidas} omitidas, {Invalidas} inválidas",
                dto.Periodo, resultado.BoletasCreadas, resultado.BoletasOmitidas, resultado.BoletasInvalidas);

            return resultado;
        }

        // Código de pago: "GD-AAAAMM-" + 12 caracteres al azar, sin repetir.
        private static string NuevoCodigoPago(string periodo, HashSet<string> codigosUsados)
        {
            string prefijo = "GD-" + periodo.Replace("/", "");
            string codigo;

            do
            {
                codigo = prefijo + "-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
            }
            while (codigosUsados.Contains(codigo));

            codigosUsados.Add(codigo);
            return codigo;
        }
    }
}
