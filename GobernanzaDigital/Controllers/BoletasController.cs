using GobernanzaDigital.DTOs;
using GobernanzaDigital.Exceptions;
using GobernanzaDigital.Interfaces;
using GobernanzaDigital.Models;
using Microsoft.AspNetCore.Mvc;

namespace GobernanzaDigital.Controllers
{
    [ApiController]
    [Route("api/contribuyentes/{contribuyenteId}/boletas")]
    public class BoletasController : ControllerBase
    {
        private readonly IBoletaService _boletaService;

        public BoletasController(IBoletaService boletaService)
        {
            _boletaService = boletaService;
        }

        // GET /api/contribuyentes/1/boletas?estado=Pendiente&periodo=2026/01
        // Los dos filtros son opcionales.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoletaReadDto>>> GetPorContribuyente(
            int contribuyenteId,
            [FromQuery] EstadoBoleta? estado,
            [FromQuery] string? periodo)
        {
            try
            {
                var boletas = await _boletaService.GetPorContribuyenteAsync(contribuyenteId, estado, periodo);
                if (boletas == null)
                {
                    return NotFound();
                }
                return Ok(boletas);
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
