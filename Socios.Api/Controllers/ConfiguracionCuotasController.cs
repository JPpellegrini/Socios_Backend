using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/configuracion-cuotas")]
    [Authorize]
    public class ConfiguracionCuotasController : ControllerBase
    {
        private readonly ITipoCuotaRepository _tipoCuotaRepository;

        public ConfiguracionCuotasController(ITipoCuotaRepository tipoCuotaRepository)
        {
            _tipoCuotaRepository = tipoCuotaRepository;
        }

        /// <summary>
        /// Trae la configuración de cuotas (tipo_cuotas + tipo_planes) para la grilla.
        /// Filtro opcional por concepto: vacío/"todos" (incluye NICHO), "socio" o "sepelio".
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarConfiguracionAsync([FromQuery] ConfiguracionCuotaFiltroDto filtro)
        {
            var configuracion = await _tipoCuotaRepository.BuscarConfiguracionAsync(filtro);

            // Una búsqueda sin coincidencias es un resultado válido: 200 con lista vacía.
            return Ok(configuracion);
        }
    }
}
