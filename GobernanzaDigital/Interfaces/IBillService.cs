using GobernanzaDigital.DTOs;

namespace GobernanzaDigital.Interfaces
{
    public interface IBillService
    {
        // null = no existe ninguna boleta con ese código de pago
        Task<BillDto?> GetPorCodigoAsync(string codigoPago);
    }
}
