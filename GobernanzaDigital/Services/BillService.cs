using GobernanzaDigital.Data;
using GobernanzaDigital.DTOs;
using GobernanzaDigital.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GobernanzaDigital.Services
{
    // Atiende las consultas de PayLink sobre una factura.
    public class BillService : IBillService
    {
        private readonly GobernanzaContext _context;
        private readonly ILogger<BillService> _logger;

        public BillService(GobernanzaContext context, ILogger<BillService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BillDto?> GetPorCodigoAsync(string codigoPago)
        {
            var boleta = await _context.Boletas
                .Include(b => b.Servicio)
                .FirstOrDefaultAsync(b => b.CodigoPago == codigoPago);

            if (boleta == null)
            {
                _logger.LogInformation("PayLink consultó una factura que no existe");
                return null;
            }

            _logger.LogInformation("PayLink consultó la boleta {BoletaId}", boleta.Id);

            return new BillDto
            {
                Id = boleta.Id,
                BillId = boleta.CodigoPago,
                Amount = boleta.Monto,
                DueDate = boleta.FechaVencimiento,
                Period = boleta.Periodo,
                ServiceName = boleta.Servicio?.Nombre ?? "",
                Status = boleta.Estado
            };
        }
    }
}
