using System.ComponentModel.DataAnnotations;

namespace GobernanzaDigital.DTOs
{
    public class GeneracionCreateDto
    {
        [Required(ErrorMessage = "El período es obligatorio.")]
        [RegularExpression(@"^\d{4}/(0[1-9]|1[0-2])$",
            ErrorMessage = "El período debe tener el formato AAAA/MM, por ejemplo 2026/11.")]
        public string Periodo { get; set; } = "";
    }
}