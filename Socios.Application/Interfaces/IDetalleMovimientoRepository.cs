using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface IDetalleMovimientoRepository
    {
        /// <summary>Trae el detalle de movimiento por su nombre exacto (ej. "CUOTA SOCIO"). Null si no existe.</summary>
        Task<DetalleMovimiento?> ObtenerPorNombreAsync(string nombre);
    }
}
