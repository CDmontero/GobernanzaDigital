using GobernanzaDigital.Models;

namespace GobernanzaDigital.DTOs
{
    public class BoletaReadDto
    {
        public int Id { get; set; }
        public int ContribuyenteId { get; set; }
        public string NombreServicio { get; set; } = "";
        public string Periodo { get; set; } = "";
        public DateTime FechaVencimiento { get; set; }
        public decimal Monto { get; set; }
        public string CodigoPago { get; set; } = "";
        public EstadoBoleta Estado { get; set; }
        public DateTime? FechaPago { get; set; }
    }
}