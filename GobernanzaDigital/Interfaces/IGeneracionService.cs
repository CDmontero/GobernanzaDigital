using GobernanzaDigital.DTOs;

namespace GobernanzaDigital.Interfaces
{
    public interface IGeneracionService
    {
        Task<GeneracionResultadoDto> GenerarBoletasAsync(GeneracionCreateDto dto);
    }
}