using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/cuotas")]
    [Authorize]
    public class CuotasController : ControllerBase
    {
        private readonly ICuotaRepository _cuotaRepository;

        public CuotasController(ICuotaRepository cuotaRepository)
        {
            _cuotaRepository = cuotaRepository;
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
    }
}
