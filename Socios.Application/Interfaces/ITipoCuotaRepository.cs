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
    }
}
