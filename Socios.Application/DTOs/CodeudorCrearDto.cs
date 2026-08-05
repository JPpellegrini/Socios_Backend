namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para dar de alta un codeudor.
    ///
    /// Hoy el alta de codeudor es, por debajo, un alta de ENTIDAD pura: hereda todos los datos
    /// de la persona (ver <see cref="EntidadCrearDto"/>). La asignación del codeudor a un socio
    /// se resuelve en un trabajo aparte, por lo que acá todavía no hay campos propios.
    /// </summary>
    public class CodeudorCrearDto : EntidadCrearDto
    {
    }
}
