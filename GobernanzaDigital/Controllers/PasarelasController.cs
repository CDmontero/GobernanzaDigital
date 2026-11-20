using GobernanzaDigital.DTOs;
using GobernanzaDigital.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GobernanzaDigital.Controllers
{
    [ApiController]
    [Route("api/pasarelas")]
    public class PasarelasController : ControllerBase
    {
        private readonly IPasarelaService _pasarelaService;

        public PasarelasController(IPasarelaService pasarelaService)
        {
            _pasarelaService = pasarelaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PasarelaReadDto>>> GetTodas()
        {
            var pasarelas = await _pasarelaService.GetTodasAsync();
            return Ok(pasarelas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PasarelaReadDto>> GetPorId(int id)
        {
            var pasarela = await _pasarelaService.GetPorIdAsync(id);
            if (pasarela == null)
            {
                return NotFound();
            }
            return Ok(pasarela);
        }

        [HttpPost]
        public async Task<ActionResult<PasarelaReadDto>> Crear(PasarelaCreateDto dto)
        {
            var pasarela = await _pasarelaService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id = pasarela.Id }, pasarela);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PasarelaReadDto>> Actualizar(int id, PasarelaCreateDto dto)
        {
            var pasarela = await _pasarelaService.ActualizarAsync(id, dto);
            if (pasarela == null)
            {
                return NotFound();
            }
            return Ok(pasarela);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var eliminada = await _pasarelaService.EliminarAsync(id);
            if (!eliminada)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
