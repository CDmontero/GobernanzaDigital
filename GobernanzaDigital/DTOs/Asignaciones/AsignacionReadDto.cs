namespace GobernanzaDigital.DTOs
{
    public class AsignacionReadDto
    {
        public int Id { get; set; }
        public int ContribuyenteId { get; set; }
        public string NombreContribuyente { get; set; } = "";
        public int ServicioId { get; set; }
        public string NombreServicio { get; set; } = "";
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public decimal? MontoPersonalizado { get; set; }
    }
}