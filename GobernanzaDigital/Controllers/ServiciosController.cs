using GobernanzaDigital.DTOs;
using GobernanzaDigital.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GobernanzaDigital.Controllers
{
    [ApiController]
    [Route("api/servicios")]
    public class ServiciosController : ControllerBase
    {
        private readonly IServicioService _servicioService;

        public ServiciosController(IServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicioReadDto>>> GetTodos()
        {
            var servicios = await _servicioService.GetTodosAsync();
            return Ok(servicios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServicioReadDto>> GetPorId(int id)
        {
            var servicio = await _servicioService.GetPorIdAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }
            return Ok(servicio);
        }

        [HttpPost]
        public async Task<ActionResult<ServicioReadDto>> Crear(ServicioCreateDto dto)
        {
            // TODO (cuando llegue ReglaNegocioException): capturarla acá con try/catch
            var servicio = await _servicioService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id = servicio.Id }, servicio);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServicioReadDto>> Actualizar(int id, ServicioCreateDto dto)
        {
            // TODO (cuando llegue ReglaNegocioException): capturarla acá con try/catch
            var servicio = await _servicioService.ActualizarAsync(id, dto);
            if (servicio == null)
            {
                return NotFound();
            }
            return Ok(servicio);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            // TODO (cuando llegue ReglaNegocioException): capturarla acá con try/catch
            var eliminado = await _servicioService.EliminarAsync(id);
            if (!eliminado)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}