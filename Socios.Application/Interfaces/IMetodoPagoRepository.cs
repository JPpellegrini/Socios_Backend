using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface IMetodoPagoRepository
    {
        /// <summary>Trae el método de pago por su Id. Null si no existe.</summary>
        Task<MetodoPago?> ObtenerPorIdAsync(int idMetodoPago);

        /// <summary>Trae el método de pago por su nombre exacto (ej. "EFECTIVO"). Null si no existe.</summary>
        Task<MetodoPago?> ObtenerPorNombreAsync(string nombre);
    }
}
