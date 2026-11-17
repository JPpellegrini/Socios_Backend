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
        /// <summary>Indica si existe una prestación con ese Id (en cualquier estado).</summary>
        Task<bool> ExisteAsync(int idPrestacion);

        /// <summary>Tipo y estado de la prestación, o null si no existe.</summary>
        Task<(string Tipo, string Estado)?> ObtenerTipoYEstadoAsync(int idPrestacion);

        /// <summary>
        /// Prestaciones ordenadas por nombre. Solo lectura. tipo / estado en null = sin filtrar
        /// por ese campo.
        /// </summary>
        Task<List<PrestacionDto>> ListarAsync(string? tipo = null, string? estado = null);

        /// <summary>La prestación trackeada, para modificarla. null si no existe.</summary>
        Task<Prestacion?> ObtenerAsync(int idPrestacion);

        /// <summary>Marca la prestación para ser insertada. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Prestacion prestacion);
    }
}
