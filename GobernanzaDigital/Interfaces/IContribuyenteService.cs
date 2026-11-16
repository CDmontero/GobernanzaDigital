using GobernanzaDigital.DTOs;

namespace GobernanzaDigital.Interfaces
{
    public interface IContribuyenteService
    {
        Task<IEnumerable<ContribuyenteReadDto>> GetTodosAsync();
        Task<ContribuyenteReadDto?> GetPorIdAsync(int id);
        Task<ContribuyenteReadDto> CrearAsync(ContribuyenteCreateDto dto);
        Task<ContribuyenteReadDto?> ActualizarAsync(int id, ContribuyenteCreateDto dto);
        Task<bool> EliminarAsync(int id);
    }
}