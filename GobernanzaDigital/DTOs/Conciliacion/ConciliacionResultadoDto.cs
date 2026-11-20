namespace GobernanzaDigital.DTOs
{
    public class ConciliacionResultadoDto
    {
        public int PasarelaId { get; set; }
        public int Consultadas { get; set; }          // boletas que se pudieron consultar
        public int Pagadas { get; set; }              // confirmadas como pagadas y actualizadas
        public int SinPagoRegistrado { get; set; }    // la pasarela respondió 404
        public int PagoNoAprobado { get; set; }       // pago Pendiente o Rechazado
        public int ConError { get; set; }             // fallaron por timeout, credenciales, etc.
        public int MarcadasVencidas { get; set; }     // pasaron a Vencida por fecha
    }
}
