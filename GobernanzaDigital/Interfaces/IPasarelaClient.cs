using GobernanzaDigital.DTOs;
using GobernanzaDigital.Models;

namespace GobernanzaDigital.Interfaces
{
    // Para hablar con PayLink (la pasarela externa).
    public interface IPasarelaClient
    {
        // Devuelve null si no hay un pago para ese código (404).
        // Lanza PasarelaException si hay timeout, clave inválida u otro error.
        Task<PagoPasarelaDto?> ConsultarPagoAsync(Pasarela pasarela, string codigoPago);
    }
}
