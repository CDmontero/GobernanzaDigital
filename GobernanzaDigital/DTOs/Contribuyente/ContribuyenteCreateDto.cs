using System.ComponentModel.DataAnnotations;
using GobernanzaDigital.Models;

namespace GobernanzaDigital.DTOs
{
    public class ContribuyenteCreateDto
    {
        [Required]
        public TipoContribuyente? Tipo { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = "";

        [Required]
        [StringLength(20)]
        public string Identificacion { get; set; } = "";

        [Required]
        [StringLength(200)]
        public string Domicilio { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}