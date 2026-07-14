using Socios.Application.Exceptions;

namespace Socios.Api.Middleware
{
    /// <summary>
    /// Middleware central de manejo de errores.
    ///
    /// Envuelve a toda la aplicación: cualquier excepción que "burbujee" desde un controlador
    /// o un caso de uso cae acá y se traduce a una respuesta HTTP prolija (un JSON con el mensaje),
    /// en vez de devolverle al cliente un error 500 crudo con el stack trace.
    ///
    ///   - ReglaNegocioException  → 409 Conflict (situación esperable, se le muestra al usuario).
    ///   - Cualquier otra          → 500 (error inesperado; se registra en el log).
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ReglaNegocioException ex)
            {
                // Regla de negocio rota: es esperable, se le informa al usuario con 409.
                await EscribirRespuesta(context, StatusCodes.Status409Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                // Error no previsto: se registra en el log y se devuelve un mensaje genérico.
                _logger.LogError(ex, "Error no controlado al procesar la solicitud.");
                await EscribirRespuesta(context, StatusCodes.Status500InternalServerError,
                    "Ocurrió un error inesperado. Contacte al administrador de Sistemas.");
            }
        }

        private static async Task EscribirRespuesta(HttpContext context, int codigo, string mensaje)
        {
            context.Response.StatusCode = codigo;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { mensaje });
        }
    }
}
