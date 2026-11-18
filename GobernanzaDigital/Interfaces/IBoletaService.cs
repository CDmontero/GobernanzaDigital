using GobernanzaDigital.DTOs;
using GobernanzaDigital.Models;

namespace GobernanzaDigital.Interfaces
{
    public interface IBoletaService
    {
        Task<IEnumerable<BoletaReadDto>?> GetPorContribuyenteAsync(int contribuyenteId, EstadoBoleta? estado, string? periodo);
    }
}