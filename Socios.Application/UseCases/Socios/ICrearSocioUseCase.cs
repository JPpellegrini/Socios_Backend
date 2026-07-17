using Socios.Application.DTOs;

namespace Socios.Application.UseCases.Socios
{
    /// <summary>
    /// Caso de uso: dar de alta un socio.
    ///
    /// Define QUÉ hace el alta (el contrato), sin atarse a CÓMO se guarda en la base.
    /// El controlador depende de esta interfaz, no de la implementación concreta
    /// (principio de inversión de dependencias: dependemos de abstracciones).
    /// </summary>
    public interface ICrearSocioUseCase
    {
        /// <summary>
        /// Ejecuta el alta y devuelve el Id del socio creado.
        /// </summary>
        Task<int> EjecutarAsync(SocioCrearDto dto);
    }
}
