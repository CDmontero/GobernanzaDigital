using System.ComponentModel.DataAnnotations;

namespace GobernanzaDigital.DTOs
{
    public class AsignacionUpdateDto
    {
        public DateTime? FechaBaja { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
        public decimal MontoBase { get; set; }
    }
}