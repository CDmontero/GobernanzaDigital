namespace GobernanzaDigital.Exceptions
{
    public class PasarelaException : Exception
    {
        public PasarelaException(string mensaje)
            : base(mensaje)
        {
        }

        // guarda el error original (por ejemplo un timeout)
        public PasarelaException(string mensaje, Exception inner)
            : base(mensaje, inner)
        {
        }
    }
}
