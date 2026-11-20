using System.Text.RegularExpressions;
using GobernanzaDigital.Data;
using GobernanzaDigital.DTOs;
using GobernanzaDigital.Exceptions;
using GobernanzaDigital.Interfaces;
using GobernanzaDigital.Models;
using Microsoft.EntityFrameworkCore;

namespace GobernanzaDigital.Services
{
    public class BoletaService : IBoletaService
    {
        private readonly GobernanzaContext _context;
        private readonly ILogger<BoletaService> _logger;

        public BoletaService(GobernanzaContext context, ILogger<BoletaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<BoletaReadDto>?> GetPorContribuyenteAsync(int contribuyenteId, EstadoBoleta? estado, string? periodo)
        {
            bool existe = await _context.Contribuyentes.AnyAsync(c => c.Id == contribuyenteId);
            if (!existe)
            {
                return null;
            }

            // validamos el formato del período (AAAA/MM)
            if (periodo != null && !Regex.IsMatch(periodo, @"^\d{4}/(0[1-9]|1[0-2])$"))
            {
                throw new ReglaNegocioException("El período debe tener el formato AAAA/MM (por ejemplo 2026/01).");
            }

            var consulta = _context.Boletas
                .Include(b => b.Servicio)
                .Where(b => b.ContribuyenteId == contribuyenteId);

            if (estado != null)
            {
                consulta = consulta.Where(b => b.Estado == estado.Value);
            }

            if (periodo != null)
            {
                consulta = consulta.Where(b => b.Periodo == periodo);
            }

            var boletas = await consulta
                .OrderByDescending(b => b.Periodo)
                .ThenBy(b => b.Id)
                .ToListAsync();

            _logger.LogInformation("Consulta de boletas del contribuyente {ContribuyenteId}: {Cantidad} resultados",
                contribuyenteId, boletas.Count);

            return boletas.Select(Mapear).ToList();
        }

        // Pasa de entidad a DTO (mapeo manual)
        private static BoletaReadDto Mapear(Boleta boleta)
        {
            return new BoletaReadDto
            {
                Id = boleta.Id,
                ContribuyenteId = boleta.ContribuyenteId,
                NombreServicio = boleta.Servicio?.Nombre ?? "",
                Periodo = boleta.Periodo,
                FechaVencimiento = boleta.FechaVencimiento,
                Monto = boleta.Monto,
                CodigoPago = boleta.CodigoPago,
                Estado = boleta.Estado,
                FechaPago = boleta.FechaPago
            };
        }
    }
}
