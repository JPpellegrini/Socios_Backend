using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    public interface ITipoCuotaRepository
    {
        /// <summary>
        /// Trae la configuración de cuotas (tipo_cuotas + tipo_planes) para la grilla,
        /// con filtro opcional por concepto.
        /// </summary>
        Task<List<ConfiguracionCuotaListadoDto>> BuscarConfiguracionAsync(ConfiguracionCuotaFiltroDto filtro);

        /// <summary>Trae el tipo de cuota por su Id (trackeado, para poder modificarlo). Null si no existe.</summary>
        Task<Domain.Entities.TipoCuota?> ObtenerPorIdAsync(int idTipoCuota);

        /// <summary>
        /// Trae el tipo de cuota del concepto SOCIO (la cuota societaria, única). Null si no existe.
        /// </summary>
        Task<Domain.Entities.TipoCuota?> ObtenerSocioAsync();

        /// <summary>
        /// Trae el tipo de cuota del concepto NICHO (para clasificar las cuotas de nicho;
        /// el monto se calcula por nicho, no sale de su importe). Null si no existe.
        /// </summary>
        Task<Domain.Entities.TipoCuota?> ObtenerNichoAsync();
    }
}
