using System.Net;
using System.Text.Json;
using GobernanzaDigital.DTOs;
using GobernanzaDigital.Exceptions;
using GobernanzaDigital.Interfaces;
using GobernanzaDigital.Models;

namespace GobernanzaDigital.Clients
{
    // Cliente que habla con PayLink. Lo que usamos de su API:
    //   GET {UrlBase}/api/payments?invoiceId={CodigoPago}   con el header X-Api-Key
    //   200 -> lista de pagos de esa factura (puede venir vacía)
    //   401 -> API Key faltante o inválida
    public class PasarelaClient : IPasarelaClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<PasarelaClient> _logger;

        private static readonly JsonSerializerOptions _opcionesJson = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PasarelaClient(HttpClient http, ILogger<PasarelaClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<PagoPasarelaDto?> ConsultarPagoAsync(Pasarela pasarela, string codigoPago)
        {
            try
            {
                string url = pasarela.UrlBase.TrimEnd('/') + "/api/payments?invoiceId=" + Uri.EscapeDataString(codigoPago);

                using var pedido = new HttpRequestMessage(HttpMethod.Get, url);
                pedido.Headers.Add("X-Api-Key", pasarela.ApiKey);   // la clave no se escribe nunca en los logs

                using var respuesta = await _http.SendAsync(pedido);

                if (respuesta.StatusCode == HttpStatusCode.NotFound)
                {
                    return null; // no hay pagos para esa factura
                }

                if (respuesta.StatusCode == HttpStatusCode.Unauthorized || respuesta.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new PasarelaException("La pasarela rechazó las credenciales (API Key inválida).");
                }

                if (!respuesta.IsSuccessStatusCode)
                {
                    throw new PasarelaException("La pasarela respondió con el código " + (int)respuesta.StatusCode + ".");
                }

                string json = await respuesta.Content.ReadAsStringAsync();
                var pagos = JsonSerializer.Deserialize<List<PagoPasarelaDto>>(json, _opcionesJson);

                if (pagos == null || pagos.Count == 0)
                {
                    return null; // la factura no tiene pagos
                }

                return ElegirPago(pagos);
            }
            catch (TaskCanceledException ex)
            {
                // se pasó el tiempo de espera (timeout)
                _logger.LogWarning("Timeout al consultar la pasarela {PasarelaId}", pasarela.Id);
                throw new PasarelaException("La pasarela tardó demasiado en responder (timeout).", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new PasarelaException("No se pudo conectar con la pasarela.", ex);
            }
            catch (JsonException ex)
            {
                throw new PasarelaException("La respuesta de la pasarela no tiene el formato esperado.", ex);
            }
            catch (UriFormatException ex)
            {
                throw new PasarelaException("La URL base de la pasarela no es válida.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new PasarelaException("La URL base de la pasarela no es válida.", ex);
            }
        }

        // Una factura puede tener varios pagos (por ejemplo uno rechazado y después uno aprobado).
        // Elegimos primero el aprobado, después el pendiente y si no el último rechazado.
        private static PagoPasarelaDto ElegirPago(List<PagoPasarelaDto> pagos)
        {
            var aprobado = pagos.FirstOrDefault(p => EsEstado(p, "APPROVED"));
            if (aprobado != null)
            {
                return aprobado;
            }

            var pendiente = pagos.FirstOrDefault(p => EsEstado(p, "PENDING"));
            if (pendiente != null)
            {
                return pendiente;
            }

            return pagos.OrderByDescending(p => p.Id).First();
        }

        private static bool EsEstado(PagoPasarelaDto pago, string estado)
        {
            return string.Equals(pago.State, estado, StringComparison.OrdinalIgnoreCase);
        }
    }
}
