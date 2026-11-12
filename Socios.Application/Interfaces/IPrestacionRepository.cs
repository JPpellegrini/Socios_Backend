using Socios.Application.DTOs;
using Socios.Domain.Entities;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Acceso a las prestaciones (catálogo de servicios).
    /// </summary>
    public interface IPrestacionRepository
    {
        /// <summary>Indica si existe una prestación con ese Id.</summary>
        Task<bool> ExisteAsync(int idPrestacion);

        /// <summary>Todas las prestaciones, ordenadas por nombre. Solo lectura.</summary>
        Task<List<PrestacionDto>> ListarAsync();

        /// <summary>Marca la prestación para ser insertada. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Prestacion prestacion);
    }
}
