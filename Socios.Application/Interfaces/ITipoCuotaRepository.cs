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
    }
}
