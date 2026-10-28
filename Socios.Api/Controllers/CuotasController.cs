using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Application.UseCases.Cuotas;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/cuotas")]
    [Authorize]
    public class CuotasController : ControllerBase
    {
        private readonly ICuotaRepository _cuotaRepository;
        private readonly ICuotaUseCase _cuotaUseCase;

        public CuotasController(ICuotaRepository cuotaRepository, ICuotaUseCase cuotaUseCase)
        {
            _cuotaRepository = cuotaRepository;
            _cuotaUseCase = cuotaUseCase;
        }

        /// <summary>
        /// Lista las cuotas para la grilla. Filtros opcionales (query string, AND):
        ///   - busqueda: campo único de socio, matchea por Nombre, Apellido o DNI.
        ///   - tipoCuota: concepto ("socio"/"sepelio"/"nicho"); vacío o "todos" no filtra.
        ///   - estado: "pagada"/"pendiente"/"vencida"/"anulada"; vacío o "todas" no filtra.
        /// Una búsqueda sin coincidencias es un resultado válido: 200 con lista vacía.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarCuotasAsync([FromQuery] CuotaFiltroDto filtro)
        {
            var cuotas = await _cuotaRepository.BuscarAsync(filtro);

            return Ok(cuotas);
        }

        /// <summary>
        /// Genera las cuotas de un socio individual en un rango de períodos (mes/año).
        /// La societaria se genera en todo el rango; la de sepelio solo hacia adelante y si
        /// el socio tiene sepelio. No genera duplicados (las que ya existen se omiten).
        /// Con un error de carga del socio (sepelio sin plan o sin nacimiento) no se genera
        /// ninguna cuota: responde 200 con el motivo en "inconsistencias".
        ///   - Validaciones de formato                       → 400 (via [ApiController]).
        ///   - Socio inexistente                             → 404 (RecursoNoEncontradoException).
        ///   - Socio inactivo / vencimiento no futuro /
        ///     desde > hasta                                 → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("generar-individual")]
        public async Task<IActionResult> GenerarIndividualAsync([FromBody] CuotaGenerarDto dto)
        {
            var resultado = await _cuotaUseCase.GenerarIndividualAsync(dto);

            return Ok(resultado);
        }

        /// <summary>
        /// Genera las cuotas de un mes/año para todos los socios activos: la societaria y,
        /// si el socio tiene sepelio y el mes es actual o futuro, la de sepelio según su plan
        /// y edad. Las que ya existen se omiten. A los socios con errores de carga no se les
        /// genera ninguna cuota y se informan en el resultado (no cortan el proceso).
        ///   - Validaciones de formato                       → 400 (via [ApiController]).
        ///   - Vencimiento no futuro / falta config. SOCIO   → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("generar-masiva")]
        public async Task<IActionResult> GenerarMasivaAsync([FromBody] CuotaGenerarMasivaDto dto)
        {
            var resultado = await _cuotaUseCase.GenerarMasivaAsync(dto);

            return Ok(resultado);
        }

        /// <summary>
        /// Anula (lógicamente) una cuota: pasa su estado a ANULADA. No borra la fila.
        /// Si es de nicho, anula todo el lote generado en la asignación y libera el nicho.
        ///   - Validaciones de formato                       → 400 (via [ApiController]).
        ///   - Cuota inexistente                             → 404 (RecursoNoEncontradoException).
        ///   - Cuota paga / ya anulada / fuera de la hora /
        ///     nicho del socio no identificable              → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("anular")]
        public async Task<IActionResult> AnularAsync([FromBody] CuotaAnularDto dto)
        {
            var resultado = await _cuotaUseCase.AnularAsync(dto);

            return Ok(resultado);
        }
    }
}
