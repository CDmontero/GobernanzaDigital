using GobernanzaDigital.Data;
using GobernanzaDigital.DTOs;
using GobernanzaDigital.Exceptions;
using GobernanzaDigital.Interfaces;
using GobernanzaDigital.Models;
using Microsoft.EntityFrameworkCore;

namespace GobernanzaDigital.Services
{
    public class ContribuyenteService : IContribuyenteService
    {
        private readonly GobernanzaContext _context;
        private readonly ILogger<ContribuyenteService> _logger;

        public ContribuyenteService(GobernanzaContext context, ILogger<ContribuyenteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ContribuyenteReadDto>> GetTodosAsync()
        {
            var contribuyentes = await _context.Contribuyentes
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return contribuyentes.Select(Mapear).ToList();
        }

        public async Task<ContribuyenteReadDto?> GetPorIdAsync(int id)
        {
            var contribuyente = await _context.Contribuyentes.FindAsync(id);
            if (contribuyente == null)
            {
                return null;
            }
            return Mapear(contribuyente);
        }

        public async Task<ContribuyenteReadDto> CrearAsync(ContribuyenteCreateDto dto)
        {
            // regla: no puede haber dos contribuyentes con el mismo DNI/CUIT
            bool yaExiste = await _context.Contribuyentes
                .AnyAsync(c => c.Identificacion == dto.Identificacion);
            if (yaExiste)
            {
                throw new ReglaNegocioException("Ya existe un contribuyente con esa identificación.", true);
            }

            var contribuyente = new Contribuyente
            {
                Tipo = dto.Tipo!.Value,
                Nombre = dto.Nombre,
                Identificacion = dto.Identificacion,
                Domicilio = dto.Domicilio,
                Email = dto.Email
            };

            _context.Contribuyentes.Add(contribuyente);
            await _context.SaveChangesAsync();

            // en el log solo ponemos el Id, nada de datos personales
            _logger.LogInformation("Contribuyente creado con Id {ContribuyenteId}", contribuyente.Id);

            return Mapear(contribuyente);
        }

        public async Task<ContribuyenteReadDto?> ActualizarAsync(int id, ContribuyenteCreateDto dto)
        {
            var contribuyente = await _context.Contribuyentes.FindAsync(id);
            if (contribuyente == null)
            {
                return null;
            }

            // la identificación no puede ser de otro contribuyente
            bool laTieneOtro = await _context.Contribuyentes
                .AnyAsync(c => c.Identificacion == dto.Identificacion && c.Id != id);
            if (laTieneOtro)
            {
                throw new ReglaNegocioException("Ya existe otro contribuyente con esa identificación.", true);
            }

            contribuyente.Tipo = dto.Tipo!.Value;
            contribuyente.Nombre = dto.Nombre;
            contribuyente.Identificacion = dto.Identificacion;
            contribuyente.Domicilio = dto.Domicilio;
            contribuyente.Email = dto.Email;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Contribuyente actualizado con Id {ContribuyenteId}", contribuyente.Id);

            return Mapear(contribuyente);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var contribuyente = await _context.Contribuyentes.FindAsync(id);
            if (contribuyente == null)
            {
                return false;
            }

            // regla: no se borra un contribuyente que ya tiene historial
            bool tieneAsignaciones = await _context.ContribuyenteServicios.AnyAsync(a => a.ContribuyenteId == id);
            bool tieneBoletas = await _context.Boletas.AnyAsync(b => b.ContribuyenteId == id);
            if (tieneAsignaciones || tieneBoletas)
            {
                throw new ReglaNegocioException("No se puede eliminar un contribuyente que tiene servicios asignados o boletas.", true);
            }

            _context.Contribuyentes.Remove(contribuyente);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Contribuyente eliminado con Id {ContribuyenteId}", id);

            return true;
        }

        // Pasa de entidad a DTO (mapeo manual)
        private static ContribuyenteReadDto Mapear(Contribuyente contribuyente)
        {
            return new ContribuyenteReadDto
            {
                Id = contribuyente.Id,
                Tipo = contribuyente.Tipo,
                Nombre = contribuyente.Nombre,
                Identificacion = contribuyente.Identificacion,
                Domicilio = contribuyente.Domicilio,
                Email = contribuyente.Email
            };
        }
    }
}
