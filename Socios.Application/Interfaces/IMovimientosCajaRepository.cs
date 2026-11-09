using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>Consultas de solo lectura para el reporte de ingresos y egresos.</summary>
    public interface IMovimientosCajaRepository
    {
        /// <summary>Tipos de actividad (detalles de movimiento), ordenados por nombre.</summary>
        Task<List<ActividadCajaDto>> ListarActividadesAsync();

        /// <summary>Nombre de la actividad, o null si no existe.</summary>
        Task<string?> ObtenerNombreActividadAsync(int idActividad);

        /// <summary>
        /// Movimientos de caja entre las dos fechas (ambas inclusive), del más antiguo al más
        /// reciente. 'tipoMovimiento' es "INGRESO", "EGRESO" o null (todos); 'idActividad' null = todas.
        /// </summary>
        Task<List<MovimientoCajaDto>> ObtenerMovimientosAsync(
            DateTime fechaDesde, DateTime fechaHasta, string? tipoMovimiento, int? idActividad);

        /// <summary>
        /// Totales de caja entre las dos fechas (ambas inclusive) agrupados por actividad y tipo de
        /// movimiento (normalizado a mayúsculas). La suma se hace en la base: no trae los movimientos.
        /// </summary>
        Task<List<TotalCajaAgrupadoDto>> ObtenerTotalesPorActividadAsync(DateTime fechaDesde, DateTime fechaHasta);
    }
}
