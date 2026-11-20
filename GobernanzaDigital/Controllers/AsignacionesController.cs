using GobernanzaDigital.DTOs;
using GobernanzaDigital.Exceptions;
using GobernanzaDigital.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GobernanzaDigital.Controllers
{
    [ApiController]
    [Route("api/asignaciones")]
    public class AsignacionesController : ControllerBase
    {
        private readonly IAsignacionService _asignacionService;

        public AsignacionesController(IAsignacionService asignacionService)
        {
            _asignacionService = asignacionService;
        }

        // GET /api/asignaciones            -> todas
        // GET /api/asignaciones?contribuyenteId=1   -> las de un contribuyente
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AsignacionReadDto>>> GetTodas([FromQuery] int? contribuyenteId)
        {
            var asignaciones = await _asignacionService.GetTodasAsync(contribuyenteId);
            return Ok(asignaciones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AsignacionReadDto>> GetPorId(int id)
        {
            var asignacion = await _asignacionService.GetPorIdAsync(id);
            if (asignacion == null)
            {
                return NotFound();
            }
            return Ok(asignacion);
        }

        [HttpPost]
        public async Task<ActionResult<AsignacionReadDto>> Crear(AsignacionCreateDto dto)
        {
            try
            {
                var asignacion = await _asignacionService.CrearAsync(dto);
                return CreatedAtAction(nameof(GetPorId), new { id = asignacion.Id }, asignacion);
            }
            catch (ReglaNegocioException ex)
            {
                return TraducirRegla(ex);
            }
        }

        // Sirve para dar de baja (FechaBaja) o cambiar el monto
        [HttpPut("{id}")]
        public async Task<ActionResult<AsignacionReadDto>> Actualizar(int id, AsignacionUpdateDto dto)
        {
            try
            {
                var asignacion = await _asignacionService.ActualizarAsync(id, dto);
                if (asignacion == null)
                {
                    return NotFound();
                }
                return Ok(asignacion);
            }
            catch (ReglaNegocioException ex)
            {
                return TraducirRegla(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var eliminada = await _asignacionService.EliminarAsync(id);
                if (!eliminada)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (ReglaNegocioException ex)
            {
                return TraducirRegla(ex);
            }
        }

        private ActionResult TraducirRegla(ReglaNegocioException ex)
        {
            if (ex.EsConflicto)
            {
                return Conflict(new { error = ex.Message });
            }
            return BadRequest(new { error = ex.Message });
        }
    }
}
