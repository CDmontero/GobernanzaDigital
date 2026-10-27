namespace GobernanzaDigital.Models
{
    public class Boleta
    {
        public int Id { get; set; }
        public int ContribuyenteId { get; set; }
        public int ServicioId { get; set; }
        public string Periodo { get; set; } = "";     // formato "2025/01"
        public DateTime FechaVencimiento { get; set; }
        public decimal Monto { get; set; }
        public string CodigoPago { get; set; } = "";  // único por boleta
        public EstadoBoleta Estado { get; set; }
        public DateTime? FechaPago { get; set; }      // null hasta que se paga

        public Contribuyente? Contribuyente { get; set; }
        public Servicio? Servicio { get; set; }
    }
}