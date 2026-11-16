using GobernanzaDigital.DTOs;

namespace GobernanzaDigital.Interfaces
{
    public interface IServicioService
    {
        Task<IEnumerable<ServicioReadDto>> GetTodosAsync();
        Task<ServicioReadDto?> GetPorIdAsync(int id);
        Task<ServicioReadDto> CrearAsync(ServicioCreateDto dto);
        Task<ServicioReadDto?> ActualizarAsync(int id, ServicioCreateDto dto);
        Task<bool> EliminarAsync(int id);
    }
}