using Socios.Application.DTOs;

namespace Socios.Application.UseCases.Prestaciones
{
    public interface IPrestacionUseCase
    {
        /// <summary>Todas las prestaciones, ordenadas por nombre.</summary>
        Task<List<PrestacionDto>> ListarAsync();

        /// <summary>
        /// Da de alta una prestación y devuelve la creada. Lanza ReglaNegocioException si ya
        /// existe una con el mismo nombre (sin distinguir mayúsculas, tildes ni espacios de más).
        /// </summary>
        Task<PrestacionDto> CrearAsync(PrestacionCrearDto dto);
    }
}
