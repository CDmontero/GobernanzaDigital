namespace GobernanzaDigital.Models
{
    public class ContribuyenteServicio
    {
        public int Id { get; set; }
        public int ContribuyenteId { get; set; }
        public int ServicioId { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }              // null = sigue activo
        public decimal? MontoPersonalizado { get; set; }      // null = usa el MontoBase

        public Contribuyente? Contribuyente { get; set; }
        public Servicio? Servicio { get; set; }
    }
}