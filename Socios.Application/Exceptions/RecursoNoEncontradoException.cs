namespace Socios.Application.Exceptions
{
    /// <summary>
    /// Se lanza cuando se pide un recurso que no existe (por ejemplo, dar de baja un socio
    /// con un Id inexistente).
    ///
    /// No es un error técnico: es una situación esperable. La capa Api la traduce a un
    /// 404 Not Found a través del middleware de errores.
    /// </summary>
    public class RecursoNoEncontradoException : Exception
    {
        public RecursoNoEncontradoException(string mensaje) : base(mensaje)
        {
        }
    }
}
