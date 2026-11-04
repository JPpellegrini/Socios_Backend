using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface ICuentaRepository
    {
        /// <summary>Trae la cuenta por su Id. Null si no existe.</summary>
        Task<Cuenta?> ObtenerPorIdAsync(int idCuenta);

        /// <summary>Trae la cuenta por su nombre exacto (ej. "EFECTIVO"). Null si no existe.</summary>
        Task<Cuenta?> ObtenerPorNombreAsync(string nombreCuenta);
    }
}
