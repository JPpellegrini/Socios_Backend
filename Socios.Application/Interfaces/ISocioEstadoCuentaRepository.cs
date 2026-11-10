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

        /// <summary>
        /// Cuotas adeudadas de un socio, o de todos si 'idEntidad' es null, opcionalmente filtradas
        /// por fecha de vencimiento (ambas inclusive; null = sin límite) y por estado
        /// (DeudasFiltroDto.Todas / Vencidas / Pendientes, con la regla de vencimiento a la fecha
        /// 'hoy'). Ordenadas por socio y vencimiento. 'Monto' y 'Estado' efectivo todavía no están calculados.
        /// </summary>
        Task<List<DeudaCuotaDto>> ObtenerDeudasAsync(int? idEntidad, DateTime? fechaDesde, DateTime? fechaHasta,
            string estadoCuota, DateTime hoy);

        /// <summary>
        /// Socios ACTIVOS con sepelio y/o nicho que, a la fecha 'hoy', adeudan como máximo
        /// 'maximoMeses' meses (meses con al menos una cuota vencida e impaga, de cualquier tipo,
        /// con la regla de vencimiento del cobro). Ordenados por apellido y nombre. 'Edad' sin calcular.
        /// </summary>
        Task<List<SocioHabilitadoSepelioDto>> ObtenerHabilitadosSepelioAsync(DateTime hoy, int maximoMeses);
    }
}
