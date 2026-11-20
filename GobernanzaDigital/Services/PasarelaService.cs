using GobernanzaDigital.Data;
using GobernanzaDigital.DTOs;
using GobernanzaDigital.Interfaces;
using GobernanzaDigital.Models;
using Microsoft.EntityFrameworkCore;

namespace GobernanzaDigital.Services
{
    public class PasarelaService : IPasarelaService
    {
        private readonly GobernanzaContext _context;
        private readonly ILogger<PasarelaService> _logger;

        public PasarelaService(GobernanzaContext context, ILogger<PasarelaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<PasarelaReadDto>> GetTodasAsync()
        {
            var lista = await _context.Pasarelas.OrderBy(p => p.Nombre).ToListAsync();
            return lista.Select(Mapear).ToList();
        }

        public async Task<PasarelaReadDto?> GetPorIdAsync(int id)
        {
            var pasarela = await _context.Pasarelas.FindAsync(id);
            if (pasarela == null)
            {
                return null;
            }
            return Mapear(pasarela);
        }

        public async Task<PasarelaReadDto> CrearAsync(PasarelaCreateDto dto)
        {
            var pasarela = new Pasarela
            {
                Nombre = dto.Nombre,
                UrlBase = dto.UrlBase,
                ApiKey = dto.ApiKey
            };

            _context.Pasarelas.Add(pasarela);
            await _context.SaveChangesAsync();

            // nunca se escribe la API Key en el log
            _logger.LogInformation("Pasarela creada con Id {PasarelaId}", pasarela.Id);

            return Mapear(pasarela);
        }

        public async Task<PasarelaReadDto?> ActualizarAsync(int id, PasarelaCreateDto dto)
        {
            var pasarela = await _context.Pasarelas.FindAsync(id);
            if (pasarela == null)
            {
                return null;
            }

            pasarela.Nombre = dto.Nombre;
            pasarela.UrlBase = dto.UrlBase;
            pasarela.ApiKey = dto.ApiKey;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Pasarela actualizada con Id {PasarelaId}", pasarela.Id);

            return Mapear(pasarela);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var pasarela = await _context.Pasarelas.FindAsync(id);
            if (pasarela == null)
            {
                return false;
            }

            _context.Pasarelas.Remove(pasarela);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pasarela eliminada con Id {PasarelaId}", id);

            return true;
        }

        // Pasa de entidad a DTO. La API Key sale enmascarada.
        private static PasarelaReadDto Mapear(Pasarela pasarela)
        {
            return new PasarelaReadDto
            {
                Id = pasarela.Id,
                Nombre = pasarela.Nombre,
                UrlBase = pasarela.UrlBase,
                ApiKeyEnmascarada = Enmascarar(pasarela.ApiKey)
            };
        }

        // deja visibles solo los últimos 4 caracteres: "****abcd"
        private static string Enmascarar(string clave)
        {
            if (string.IsNullOrEmpty(clave) || clave.Length <= 4)
            {
                return "****";
            }
            return "****" + clave.Substring(clave.Length - 4);
        }
    }
}
