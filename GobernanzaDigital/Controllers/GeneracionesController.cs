using GobernanzaDigital.DTOs;
using GobernanzaDigital.Exceptions;
using GobernanzaDigital.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GobernanzaDigital.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   // /api/generaciones
    public class GeneracionesController : ControllerBase
    {
        private readonly IGeneracionService _servicio;

        public GeneracionesController(IGeneracionService servicio)
        {
            _servicio = servicio;
        }

        // POST /api/generaciones   { "periodo": "2026/01" }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<GeneracionResultadoDto>> Generar([FromBody] GeneracionCreateDto dto)
        {
            try
            {
                var resultado = await _servicio.GenerarBoletasAsync(dto);
                return Ok(resultado);
            }
            catch (ReglaNegocioException ex)
            {
                if (ex.EsConflicto)
                {
                    return Conflict(new { error = ex.Message });
                }
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
