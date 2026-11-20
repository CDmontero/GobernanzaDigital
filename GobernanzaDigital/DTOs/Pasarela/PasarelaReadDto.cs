namespace GobernanzaDigital.DTOs
{
    public class PasarelaReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string UrlBase { get; set; } = "";

        // La API Key es una clave secreta: nunca se devuelve completa
        public string ApiKeyEnmascarada { get; set; } = "";
    }
}
