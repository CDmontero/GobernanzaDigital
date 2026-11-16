using GobernanzaDigital.DTOs;

namespace GobernanzaDigital.Interfaces
{
    public interface IAsignacionService
    {
        Task<IEnumerable<AsignacionReadDto>> GetTodasAsync(int? contribuyenteId);
        Task<AsignacionReadDto?> GetPorIdAsync(int id);
        Task<AsignacionReadDto> CrearAsync(AsignacionCreateDto dto);
        Task<AsignacionReadDto?> ActualizarAsync(int id, AsignacionUpdateDto dto);
        Task<bool> EliminarAsync(int id);
    }
}