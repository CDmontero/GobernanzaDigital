namespace GobernanzaDigital.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int FrecuenciaEnMeses { get; set; }   // 1 mensual, 2 bimestral, 12 anual
        public decimal MontoBase { get; set; }
    }
}