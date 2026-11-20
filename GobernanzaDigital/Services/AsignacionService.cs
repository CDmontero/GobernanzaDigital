using GobernanzaDigital.Data;
using GobernanzaDigital.DTOs;
using GobernanzaDigital.Exceptions;
using GobernanzaDigital.Interfaces;
using GobernanzaDigital.Models;
using Microsoft.EntityFrameworkCore;

namespace GobernanzaDigital.Services
{
    public class AsignacionService : IAsignacionService
    {
        private readonly GobernanzaContext _context;
        private readonly ILogger<AsignacionService> _logger;

        public AsignacionService(GobernanzaContext context, ILogger<AsignacionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<AsignacionReadDto>> GetTodasAsync(int? contribuyenteId)
        {
            var consulta = _context.ContribuyenteServicios
                .Include(a => a.Contribuyente)
                .Include(a => a.Servicio)
                .AsQueryable();

            if (contribuyenteId != null)
            {
                consulta = consulta.Where(a => a.ContribuyenteId == contribuyenteId.Value);
            }

            var asignaciones = await consulta.OrderBy(a => a.Id).ToListAsync();
            return asignaciones.Select(Mapear).ToList();
        }

        public async Task<AsignacionReadDto?> GetPorIdAsync(int id)
        {
            var asignacion = await _context.ContribuyenteServicios
                .Include(a => a.Contribuyente)
                .Include(a => a.Servicio)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asignacion == null)
            {
                return null;
            }
            return Mapear(asignacion);
        }

        public async Task<AsignacionReadDto> CrearAsync(AsignacionCreateDto dto)
        {
            int contribuyenteId = dto.ContribuyenteId!.Value;
            int servicioId = dto.ServicioId!.Value;

            var contribuyente = await _context.Contribuyentes.FindAsync(contribuyenteId);
            if (contribuyente == null)
            {
                throw new ReglaNegocioException("El contribuyente indicado no existe.");
            }

            var servicio = await _context.Servicios.FindAsync(servicioId);
            if (servicio == null)
            {
                throw new ReglaNegocioException("El servicio indicado no existe.");
            }

            // no puede tener dos veces el mismo servicio
            bool yaAsignado = await _context.ContribuyenteServicios
                .AnyAsync(a => a.ContribuyenteId == contribuyenteId && a.ServicioId == servicioId);
            if (yaAsignado)
            {
                throw new ReglaNegocioException("El contribuyente ya tiene asignado ese servicio.", true);
            }

            var asignacion = new ContribuyenteServicio
            {
                ContribuyenteId = contribuyenteId,
                ServicioId = servicioId,
                FechaAlta = dto.FechaAlta!.Value,
                FechaBaja = null,
                MontoPersonalizado = CalcularMontoPersonalizado(dto.MontoBase, servicio)
            };

            _context.ContribuyenteServicios.Add(asignacion);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Servicio {ServicioId} asignado al contribuyente {ContribuyenteId}",
                servicioId, contribuyenteId);

            asignacion.Contribuyente = contribuyente;   // para mostrar los nombres en la respuesta
            asignacion.Servicio = servicio;
            return Mapear(asignacion);
        }

        public async Task<AsignacionReadDto?> ActualizarAsync(int id, AsignacionUpdateDto dto)
        {
            var asignacion = await _context.ContribuyenteServicios
                .Include(a => a.Contribuyente)
                .Include(a => a.Servicio)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asignacion == null)
            {
                return null;
            }

            // la baja no puede ser antes del alta
            if (dto.FechaBaja != null && dto.FechaBaja.Value < asignacion.FechaAlta)
            {
                throw new ReglaNegocioException("La fecha de baja no puede ser anterior a la fecha de alta.");
            }

            asignacion.FechaBaja = dto.FechaBaja;
            asignacion.MontoPersonalizado = CalcularMontoPersonalizado(dto.MontoBase, asignacion.Servicio!);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Asignación actualizada con Id {AsignacionId}", asignacion.Id);

            return Mapear(asignacion);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var asignacion = await _context.ContribuyenteServicios.FindAsync(id);
            if (asignacion == null)
            {
                return false;
            }

            // Si ya hay boletas no se borra, para no perder el historial:
            // hay que dar de baja con una fecha de baja (PUT)
            bool tieneBoletas = await _context.Boletas
                .AnyAsync(b => b.ContribuyenteId == asignacion.ContribuyenteId && b.ServicioId == asignacion.ServicioId);
            if (tieneBoletas)
            {
                throw new ReglaNegocioException("La asignación ya tiene boletas generadas. Dala de baja con una fecha de baja en lugar de eliminarla.", true);
            }

            _context.ContribuyenteServicios.Remove(asignacion);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Asignación eliminada con Id {AsignacionId}", id);

            return true;
        }

        // Si el monto es igual al del servicio no lo guardamos como personalizado (queda null)
        private static decimal? CalcularMontoPersonalizado(decimal monto, Servicio servicio)
        {
            if (monto == servicio.MontoBase)
            {
                return null;
            }
            return monto;
        }

        // Pasa de entidad a DTO (mapeo manual)
        private static AsignacionReadDto Mapear(ContribuyenteServicio asignacion)
        {
            return new AsignacionReadDto
            {
                Id = asignacion.Id,
                ContribuyenteId = asignacion.ContribuyenteId,
                NombreContribuyente = asignacion.Contribuyente?.Nombre ?? "",
                ServicioId = asignacion.ServicioId,
                NombreServicio = asignacion.Servicio?.Nombre ?? "",
                FechaAlta = asignacion.FechaAlta,
                FechaBaja = asignacion.FechaBaja,
                MontoPersonalizado = asignacion.MontoPersonalizado
            };
        }
    }
}
