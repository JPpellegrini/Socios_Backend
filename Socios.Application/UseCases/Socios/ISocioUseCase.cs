using Socios.Application.DTOs;

namespace Socios.Application.UseCases.Socios
{
    /// <summary>
    /// Punto de entrada de los casos de uso del socio (alta, visualización, etc.).
    ///
    /// Define QUÉ operaciones existen, sin atarse a CÓMO se resuelven por dentro.
    /// El controlador depende de esta interfaz, no de la implementación concreta
    /// (principio de inversión de dependencias: dependemos de abstracciones).
    /// </summary>
    public interface ISocioUseCase
    {
        /// <summary>Da de alta un socio y devuelve el Id generado.</summary>
        Task<int> CrearAsync(SocioCrearDto dto);

        /// <summary>
        /// Trae todos los datos de un socio para visualizarlo / editarlo.
        /// Devuelve null si no existe.
        /// </summary>
        Task<SocioDetalleDto?> VisualizarAsync(int idSocio);
    }
}
