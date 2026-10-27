namespace GobernanzaDigital.Models
{
    public class Pasarela
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string UrlBase { get; set; } = "";
        public string ApiKey { get; set; } = "";
    }
}