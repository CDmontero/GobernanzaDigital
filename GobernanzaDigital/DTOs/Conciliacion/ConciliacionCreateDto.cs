using System.ComponentModel.DataAnnotations;

namespace GobernanzaDigital.DTOs
{
    public class ConciliacionCreateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "El PasarelaId es obligatorio.")]
        public int PasarelaId { get; set; }
    }
}
