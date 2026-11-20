using GobernanzaDigital.DTOs;
using GobernanzaDigital.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GobernanzaDigital.Controllers
{
    // Endpoint que usa PayLink para consultar una factura por su código de pago.
    // La ruta es /bills (sin api/) porque así la pide PayLink.
    [ApiController]
    [Route("bills")]
    public class BillsController : ControllerBase
    {
        private readonly IBillService _billService;

        public BillsController(IBillService billService)
        {
            _billService = billService;
        }

        // GET /bills/GD-202601-3F9A1C07B2D4
        [HttpGet("{billId}")]
        public async Task<ActionResult<BillDto>> GetPorCodigo(string billId)
        {
            var bill = await _billService.GetPorCodigoAsync(billId);
            if (bill == null)
            {
                return NotFound();
            }
            return Ok(bill);
        }
    }
}
