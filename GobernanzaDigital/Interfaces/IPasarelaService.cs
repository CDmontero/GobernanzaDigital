using GobernanzaDigital.DTOs;

namespace GobernanzaDigital.Interfaces
{
    public interface IPasarelaService
    {
        Task<IEnumerable<PasarelaReadDto>> GetTodasAsync();
        Task<PasarelaReadDto?> GetPorIdAsync(int id);
        Task<PasarelaReadDto> CrearAsync(PasarelaCreateDto dto);
        Task<PasarelaReadDto?> ActualizarAsync(int id, PasarelaCreateDto dto);   // null = no existe
        Task<bool> EliminarAsync(int id);                                        // false = no existe
    }
}
