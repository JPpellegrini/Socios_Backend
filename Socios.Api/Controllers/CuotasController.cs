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
        ///   - Validaciones de formato                  → 400 (via [ApiController]).
        ///   - Socio inexistente                        → 404 (RecursoNoEncontradoException).
        ///   - Vencimiento no futuro / desde > hasta /
        ///     sepelio sin plan o sin nacimiento        → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("generar-individual")]
        public async Task<IActionResult> GenerarIndividualAsync([FromBody] CuotaGenerarDto dto)
        {
            var resultado = await _cuotaUseCase.GenerarIndividualAsync(dto);

            return Ok(resultado);
        }

        /// <summary>
        /// Anula (lógicamente) una cuota: pasa su estado a ANULADA. No borra la fila.
        ///   - Validaciones de formato                       → 400 (via [ApiController]).
        ///   - Cuota inexistente                             → 404 (RecursoNoEncontradoException).
        ///   - Cuota paga / ya anulada / fuera de la hora    → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("anular")]
        public async Task<IActionResult> AnularAsync([FromBody] CuotaAnularDto dto)
        {
            await _cuotaUseCase.AnularAsync(dto);

            return Ok(new { mensaje = "La cuota fue anulada correctamente." });
        }
    }
}
