namespace GobernanzaDigital.DTOs
{
    public class GeneracionResultadoDto
    {
        public string Periodo { get; set; } = "";
        public int BoletasCreadas { get; set; }
        public int BoletasOmitidas { get; set; }   // ya existían, no se duplican
    }
}