using GobernanzaDigital.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GobernanzaDigital.Controllers
{
    // Simulador de PayLink, solo para probar en desarrollo.
    // Sirve para probar la conciliación sin depender del otro grupo.
    // Para usarlo, creá una pasarela con UrlBase = https://localhost:7233/mock-paylink y ApiKey = clave-demo.
    // Responde según el último carácter del código de pago:
    //   0-7 aprobado, 8-9 pendiente, A-C rechazado, D-F sin pagos
    [ApiController]
    [Route("mock-paylink/api/payments")]
    public class MockPayLinkController : ControllerBase
    {
        private const string ClaveDemo = "clave-demo";

        private readonly GobernanzaContext _context;
        private readonly IWebHostEnvironment _entorno;

        public MockPayLinkController(GobernanzaContext context, IWebHostEnvironment entorno)
        {
            _context = context;
            _entorno = entorno;
        }

        // GET /mock-paylink/api/payments?invoiceId={codigoPago}
        [HttpGet]
        public async Task<IActionResult> GetPagos([FromQuery] string invoiceId)
        {
            if (!_entorno.IsDevelopment())
            {
                return NotFound();
            }

            if (Request.Headers["X-Api-Key"] != ClaveDemo)
            {
                return Unauthorized(new { title = "API Key faltante o inválida.", status = 401 });
            }

            var boleta = await _context.Boletas.FirstOrDefaultAsync(b => b.CodigoPago == invoiceId);
            if (boleta == null || string.IsNullOrEmpty(invoiceId))
            {
                return Ok(new object[0]);
            }

            char ultimo = char.ToUpper(invoiceId[invoiceId.Length - 1]);
            string estado;
            if ("01234567".Contains(ultimo))
            {
                estado = "APPROVED";
            }
            else if ("89".Contains(ultimo))
            {
                estado = "PENDING";
            }
            else if ("ABC".Contains(ultimo))
            {
                estado = "REJECTED";
            }
            else
            {
                return Ok(new object[0]);
            }

            return Ok(new[]
            {
                new
                {
                    id = boleta.Id,
                    invoiceId = invoiceId,
                    amount = boleta.Monto,
                    state = estado,
                    paymentDate = (DateTime?)DateTime.UtcNow
                }
            });
        }
    }
}
