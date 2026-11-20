using GobernanzaDigital.DTOs;

namespace GobernanzaDigital.Interfaces
{
    public interface IConciliacionService
    {
        // null = la pasarela no existe
        Task<ConciliacionResultadoDto?> ConciliarAsync(ConciliacionCreateDto dto);
    }
}
