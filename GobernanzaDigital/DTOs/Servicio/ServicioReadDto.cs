namespace GobernanzaDigital.DTOs
{
    public class ServicioReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int FrecuenciaEnMeses { get; set; }
        public decimal MontoBase { get; set; }
    }
}