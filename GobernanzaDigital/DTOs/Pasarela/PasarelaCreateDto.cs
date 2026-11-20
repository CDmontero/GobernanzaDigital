using System.ComponentModel.DataAnnotations;

namespace GobernanzaDigital.DTOs
{
    // Sirve para crear (POST) y para actualizar (PUT) una pasarela
    public class PasarelaCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = "";

        [Required]
        [Url(ErrorMessage = "La URL base no es válida (ejemplo: https://paylink.ejemplo.com).")]
        [StringLength(300)]
        public string UrlBase { get; set; } = "";

        [Required]
        [StringLength(200)]
        public string ApiKey { get; set; } = "";
    }
}
