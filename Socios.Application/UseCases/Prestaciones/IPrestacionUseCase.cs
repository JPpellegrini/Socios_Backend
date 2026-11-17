using Socios.Application.DTOs;

namespace Socios.Application.UseCases.Prestaciones
{
    public interface IPrestacionUseCase
    {
        /// <summary>Prestaciones ordenadas por nombre, filtradas por tipo y estado.</summary>
        Task<List<PrestacionDto>> ListarAsync(PrestacionFiltroDto filtro);

        /// <summary>
        /// Da de alta una prestación (en estado ACTIVA) y devuelve la creada. Lanza
        /// ReglaNegocioException si ya existe una con el mismo nombre (sin distinguir
        /// mayúsculas, tildes ni espacios de más).
        /// </summary>
        Task<PrestacionDto> CrearAsync(PrestacionCrearDto dto);

        /// <summary>
        /// Cambia el nombre y el tipo de una prestación. RecursoNoEncontradoException si no
        /// existe; ReglaNegocioException si el nombre nuevo ya lo usa otra prestación.
        /// </summary>
        Task<PrestacionDto> ModificarAsync(int idPrestacion, PrestacionModificarDto dto);

        /// <summary>
        /// Pasa la prestación a INACTIVA (baja lógica). RecursoNoEncontradoException si no existe;
        /// ReglaNegocioException si ya estaba inactiva.
        /// </summary>
        Task<PrestacionDto> DarDeBajaAsync(int idPrestacion);

        /// <summary>
        /// Pasa la prestación a ACTIVA. RecursoNoEncontradoException si no existe;
        /// ReglaNegocioException si ya estaba activa.
        /// </summary>
        Task<PrestacionDto> ReactivarAsync(int idPrestacion);
    }
}
