using System.ComponentModel.DataAnnotations;

namespace GobernanzaDigital.DTOs
{
    public class AsignacionUpdateDto
    {
        public DateTime? FechaBaja { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal? MontoPersonalizado { get; set; }
    }
}