using Socios.Application.DTOs;

namespace Socios.Application.UseCases.Entidades
{
    /// <summary>
    /// Punto de entrada de los casos de uso de la entidad (persona).
    ///
    /// Define QUÉ operaciones existen, sin atarse a CÓMO se resuelven por dentro.
    /// Otros casos de uso (alta de codeudor, etc.) reutilizan estas operaciones.
    /// </summary>
    public interface IEntidadUseCase
    {
        /// <summary>
        /// Alta de entidad pura (con sus contactos). Devuelve el Id de la entidad.
        /// Si ya existe una entidad con ese DNI, la reutiliza en vez de duplicarla.
        /// </summary>
        Task<int> CrearAsync(EntidadCrearDto dto);
    }
}
