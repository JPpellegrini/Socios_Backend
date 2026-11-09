using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>Consultas de solo lectura para el reporte de movimientos por entidad.</summary>
    public interface IMovimientosEntidadRepository
    {
        /// <summary>
        /// Entidades del tipo indicado (activas e inactivas) para el selector del filtro,
        /// ordenadas por apellido, nombre y razón social.
        /// </summary>
        Task<List<EntidadMovimientosDto>> ListarEntidadesAsync(TipoEntidadMovimientos tipo);

        /// <summary>Datos de la entidad para el encabezado. Null si la entidad no es de ese tipo.</summary>
        Task<EntidadMovimientosDto?> ObtenerEntidadAsync(int idEntidad, TipoEntidadMovimientos tipo);

        /// <summary>
        /// Movimientos de caja de la entidad con los conceptos del tipo indicado, entre las dos
        /// fechas (ambas inclusive), del más antiguo al más reciente.
        /// </summary>
        Task<List<MovimientoEntidadDto>> ObtenerMovimientosAsync(
            int idEntidad, TipoEntidadMovimientos tipo, DateTime fechaDesde, DateTime fechaHasta);
    }
}
