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
    }
}
