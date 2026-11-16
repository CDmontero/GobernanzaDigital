using System.ComponentModel.DataAnnotations;

namespace GobernanzaDigital.DTOs
{
    public class AsignacionCreateDto
    {
        [Required]
        public int? ContribuyenteId { get; set; }

        [Required]
        public int? ServicioId { get; set; }

        [Required]
        public DateTime? FechaAlta { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
        public decimal MontoBase { get; set; }
    }
}