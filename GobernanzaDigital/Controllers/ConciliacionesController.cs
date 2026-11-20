using GobernanzaDigital.DTOs;
using GobernanzaDigital.Exceptions;
using GobernanzaDigital.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GobernanzaDigital.Controllers
{
    [ApiController]
    [Route("api/conciliaciones")]
    public class ConciliacionesController : ControllerBase
    {
        private readonly IConciliacionService _conciliacionService;

        public ConciliacionesController(IConciliacionService conciliacionService)
        {
            _conciliacionService = conciliacionService;
        }

        // POST /api/conciliaciones   { "pasarelaId": 1 }
        [HttpPost]
        public async Task<ActionResult<ConciliacionResultadoDto>> Conciliar(ConciliacionCreateDto dto)
        {
            try
            {
                var resultado = await _conciliacionService.ConciliarAsync(dto);
                if (resultado == null)
                {
                    return NotFound(new { error = "La pasarela indicada no existe." });
                }
                return Ok(resultado);
            }
            catch (PasarelaException ex)
            {
                // falló la conexión con la pasarela (timeout, clave inválida, etc.): devolvemos 502
                return StatusCode(StatusCodes.Status502BadGateway, new { error = ex.Message });
            }
        }
    }
}
