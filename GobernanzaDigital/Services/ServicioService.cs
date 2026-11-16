using GobernanzaDigital.Data;
using GobernanzaDigital.DTOs;
using GobernanzaDigital.Interfaces;
using GobernanzaDigital.Models;
using Microsoft.EntityFrameworkCore;

namespace GobernanzaDigital.Services
{
    public class ServicioService : IServicioService
    {
        private readonly GobernanzaContext _context;
        private readonly ILogger<ServicioService> _logger;

        public ServicioService(GobernanzaContext context, ILogger<ServicioService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ServicioReadDto>> GetTodosAsync()
        {
            return await _context.Servicios
                .Select(s => new ServicioReadDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    FrecuenciaEnMeses = s.FrecuenciaEnMeses,
                    MontoBase = s.MontoBase
                })
                .ToListAsync();
        }

        public async Task<ServicioReadDto?> GetPorIdAsync(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return null;
            }
            return Mapear(servicio);
        }

        public async Task<ServicioReadDto> CrearAsync(ServicioCreateDto dto)
        {
            // TODO (cuando llegue ReglaNegocioException):
            // - validar que la frecuencia sea 1, 2, 3, 4, 6 o 12
            // - validar que no exista otro servicio con el mismo Nombre (conflicto: true)

            var servicio = new Servicio
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                FrecuenciaEnMeses = dto.FrecuenciaEnMeses,
                MontoBase = dto.MontoBase
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Servicio creado con Id {ServicioId}", servicio.Id);

            return Mapear(servicio);
        }

        public async Task<ServicioReadDto?> ActualizarAsync(int id, ServicioCreateDto dto)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return null;
            }

            // TODO (cuando llegue ReglaNegocioException):
            // - validar frecuencia (1, 2, 3, 4, 6 o 12)
            // - validar que el Nombre no lo tenga OTRO servicio (conflicto: true)

            servicio.Nombre = dto.Nombre;
            servicio.Descripcion = dto.Descripcion;
            servicio.FrecuenciaEnMeses = dto.FrecuenciaEnMeses;
            servicio.MontoBase = dto.MontoBase;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Servicio actualizado con Id {ServicioId}", servicio.Id);

            return Mapear(servicio);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return false;
            }

            // TODO (cuando llegue ReglaNegocioException):
            // - no eliminar si tiene asignaciones o boletas (conflicto: true)

            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Servicio eliminado con Id {ServicioId}", id);

            return true;
        }

        // Pasa de entidad a DTO (mapeo manual)
        private static ServicioReadDto Mapear(Servicio servicio)
        {
            return new ServicioReadDto
            {
                Id = servicio.Id,
                Nombre = servicio.Nombre,
                Descripcion = servicio.Descripcion,
                FrecuenciaEnMeses = servicio.FrecuenciaEnMeses,
                MontoBase = servicio.MontoBase
            };
        }
    }
}