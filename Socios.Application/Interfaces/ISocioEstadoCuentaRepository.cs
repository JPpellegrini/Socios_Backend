using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>Consultas de solo lectura para el estado de cuenta del socio.</summary>
    public interface ISocioEstadoCuentaRepository
    {
        /// <summary>
        /// Socios (activos e inactivos) para el selector, ordenados por apellido y nombre.
        /// 'busqueda' (opcional) filtra por nombre, apellido o DNI.
        /// </summary>
        Task<List<SocioSeleccionDto>> ListarSociosAsync(string? busqueda);

        /// <summary>Datos del socio de una entidad. Null si la entidad no es socio.</summary>
        Task<SocioSeleccionDto?> ObtenerSocioAsync(int idEntidad);

        /// <summary>
        /// Cuotas no anuladas de la entidad con vencimiento entre las dos fechas (inclusive),
        /// con los datos del pago si lo tienen. 'Monto' todavía no está calculado.
        /// </summary>
        Task<List<SocioCuotaCuentaDto>> ObtenerCuotasAsync(int idEntidad, DateTime fechaDesde, DateTime fechaHasta);

        /// <summary>Cuotas adeudadas (pendientes y vencidas) de la entidad, sin filtro de fechas.</summary>
        Task<List<SocioCuotaCuentaDto>> ObtenerCuotasAdeudadasAsync(int idEntidad);
    }
}
