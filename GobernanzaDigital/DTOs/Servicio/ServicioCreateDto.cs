using System.ComponentModel.DataAnnotations;

namespace GobernanzaDigital.DTOs
{
    public class ServicioCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = "";

        [StringLength(300)]
        public string Descripcion { get; set; } = "";

        [Range(1, 12)]
        public int FrecuenciaEnMeses { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal MontoBase { get; set; }
    }
}