namespace GobernanzaDigital.Exceptions
{
    public class ReglaNegocioException : Exception
    {
        // true = conflicto (409), false = pedido inválido (400)
        public bool EsConflicto { get; }

        public ReglaNegocioException(string mensaje, bool esConflicto = false)
            : base(mensaje)
        {
            EsConflicto = esConflicto;
        }
    }
}
