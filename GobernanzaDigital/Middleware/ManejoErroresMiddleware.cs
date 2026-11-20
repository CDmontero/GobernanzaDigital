namespace GobernanzaDigital.Middleware
{
    // Atrapa cualquier error que no esperábamos
    // y devuelve un 500 genérico, sin mostrar detalles internos.
    public class ManejoErroresMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ManejoErroresMiddleware> _logger;

        public ManejoErroresMiddleware(RequestDelegate next, ILogger<ManejoErroresMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // sigue con la petición
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado en {Metodo} {Ruta}",
                    context.Request.Method, context.Request.Path);

                if (context.Response.HasStarted)
                {
                    throw; // ya empezó a responder, no se puede cambiar
                }

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "Ocurrió un error inesperado en el servidor." });
            }
        }
    }
}
