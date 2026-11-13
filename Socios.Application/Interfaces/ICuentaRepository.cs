using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface ICuentaRepository
    {
        /// <summary>Trae la cuenta por su Id. Null si no existe.</summary>
        Task<Cuenta?> ObtenerPorIdAsync(int idCuenta);

        /// <summary>Trae la cuenta por su nombre exacto (ej. "EFECTIVO"). Null si no existe.</summary>
        Task<Cuenta?> ObtenerPorNombreAsync(string nombreCuenta);

        /// <summary>Cuentas ACTIVAS ordenadas por nombre (para los desplegables). Solo lectura.</summary>
        Task<List<Cuenta>> ListarActivasAsync();
    }
}
