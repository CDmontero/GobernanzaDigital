namespace GobernanzaDigital.Models
{
    public class Contribuyente
    {
        public int Id { get; set; }
        public TipoContribuyente Tipo { get; set; }
        public string Nombre { get; set; } = "";
        public string Identificacion { get; set; } = "";   // DNI o CUIT
        public string Domicilio { get; set; } = "";
        public string Email { get; set; } = "";

        public List<ContribuyenteServicio> Asignaciones { get; set; } = new();
        public List<Boleta> Boletas { get; set; } = new();
    }
}