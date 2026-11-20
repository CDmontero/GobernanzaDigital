using GobernanzaDigital.DTOs;
using GobernanzaDigital.Exceptions;
using GobernanzaDigital.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GobernanzaDigital.Controllers
{
    [ApiController]
    [Route("api/contribuyentes")]
    public class ContribuyentesController : ControllerBase
    {
        private readonly IContribuyenteService _contribuyenteService;

        public ContribuyentesController(IContribuyenteService contribuyenteService)
        {
            _contribuyenteService = contribuyenteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContribuyenteReadDto>>> GetTodos()
        {
            var contribuyentes = await _contribuyenteService.GetTodosAsync();
            return Ok(contribuyentes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContribuyenteReadDto>> GetPorId(int id)
        {
            var contribuyente = await _contribuyenteService.GetPorIdAsync(id);
            if (contribuyente == null)
            {
                return NotFound();
            }
            return Ok(contribuyente);
        }

        [HttpPost]
        public async Task<ActionResult<ContribuyenteReadDto>> Crear(ContribuyenteCreateDto dto)
        {
            try
            {
                var contribuyente = await _contribuyenteService.CrearAsync(dto);
                return CreatedAtAction(nameof(GetPorId), new { id = contribuyente.Id }, contribuyente);
            }
            catch (ReglaNegocioException ex)
            {
                return TraducirRegla(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ContribuyenteReadDto>> Actualizar(int id, ContribuyenteCreateDto dto)
        {
            try
            {
                var contribuyente = await _contribuyenteService.ActualizarAsync(id, dto);
                if (contribuyente == null)
                {
                    return NotFound();
                }
                return Ok(contribuyente);
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
                var eliminado = await _contribuyenteService.EliminarAsync(id);
                if (!eliminado)
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

        // el servicio lanza la excepción y acá la pasamos a 409 o 400
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
