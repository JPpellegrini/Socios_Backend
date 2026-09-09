using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Application.UseCases.ConfiguracionCuotas;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/configuracion-cuotas")]
    [Authorize]
    public class ConfiguracionCuotasController : ControllerBase
    {
        private readonly ITipoCuotaRepository _tipoCuotaRepository;
        private readonly IConfiguracionCuotaUseCase _configuracionCuotaUseCase;

        public ConfiguracionCuotasController(
            ITipoCuotaRepository tipoCuotaRepository,
            IConfiguracionCuotaUseCase configuracionCuotaUseCase)
        {
            _tipoCuotaRepository = tipoCuotaRepository;
            _configuracionCuotaUseCase = configuracionCuotaUseCase;
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

        /// <summary>
        /// Modifica el importe de la cuota del concepto SOCIO. La fecha de última
        /// modificación se actualiza automáticamente.
        ///   - Validaciones de formato          → 400 (via [ApiController]).
        ///   - Tipo de cuota inexistente        → 404 (RecursoNoEncontradoException).
        ///   - El tipo de cuota no es de SOCIO   → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPut("modificar-socio")]
        public async Task<IActionResult> ModificarImporteSocioAsync([FromBody] ConfiguracionCuotaModificarDto dto)
        {
            await _configuracionCuotaUseCase.ModificarImporteSocioAsync(dto);

            return Ok(new { mensaje = "El importe de la cuota de socio fue modificado correctamente." });
        }

        /// <summary>
        /// Modifica la cuota de SEPELIO "hasta el tope": importe + tope de edad. La fecha de
        /// última modificación se actualiza automáticamente. El tope editado también corre la
        /// frontera que muestra la cuota "más de" del mismo plan.
        ///   - Validaciones de formato                    → 400 (via [ApiController]).
        ///   - Tipo de cuota inexistente                  → 404 (RecursoNoEncontradoException).
        ///   - No es SEPELIO, o es la cuota "más de"       → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPut("modificar-sepelio-con-tope")]
        public async Task<IActionResult> ModificarSepelioHastaAsync([FromBody] SepelioHastaModificarDto dto)
        {
            await _configuracionCuotaUseCase.ModificarSepelioHastaAsync(dto);

            return Ok(new { mensaje = "La cuota de sepelio (con tope) fue modificada correctamente." });
        }

        /// <summary>
        /// Modifica la cuota de SEPELIO "más de" (sin tope de edad propio): solo el importe.
        /// La fecha de última modificación se actualiza automáticamente.
        ///   - Validaciones de formato                       → 400 (via [ApiController]).
        ///   - Tipo de cuota inexistente                     → 404 (RecursoNoEncontradoException).
        ///   - No es SEPELIO, o es la cuota "hasta el tope"   → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPut("modificar-sepelio-sin-tope")]
        public async Task<IActionResult> ModificarSepelioMasDeAsync([FromBody] SepelioMasDeModificarDto dto)
        {
            await _configuracionCuotaUseCase.ModificarSepelioMasDeAsync(dto);

            return Ok(new { mensaje = "La cuota de sepelio (más de) fue modificada correctamente." });
        }
    }
}
