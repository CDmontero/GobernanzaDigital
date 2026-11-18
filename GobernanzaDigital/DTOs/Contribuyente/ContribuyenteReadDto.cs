using GobernanzaDigital.Models;

namespace GobernanzaDigital.DTOs
{
    public class ContribuyenteReadDto
    {
        public int Id { get; set; }
        public TipoContribuyente Tipo { get; set; }
        public string Nombre { get; set; } = "";
        public string Identificacion { get; set; } = "";
        public string Domicilio { get; set; } = "";
        public string Email { get; set; } = "";
    }
}