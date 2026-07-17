namespace Socios.Application.Exceptions
{
    /// <summary>
    /// Se lanza cuando se rompe una regla del negocio (por ejemplo: intentar dar de alta
    /// un socio que ya existe).
    ///
    /// No es un error técnico ni un bug: es una situación esperable que hay que avisarle
    /// al usuario. La capa Api la traduce a una respuesta HTTP clara (409 Conflict) a
    /// través del middleware de errores, así el caso de uso no tiene que saber nada de HTTP.
    /// </summary>
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje)
        {
        }
    }
}
