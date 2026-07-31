using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/obrassociales")]
    [Authorize]
    public class ObrasSocialesController : ControllerBase
    {
        private readonly IObraSocialRepository _obraSocialRepository;

        public ObrasSocialesController(IObraSocialRepository obraRepository)
        {
            _obraSocialRepository = obraRepository;
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarObrasSocialesAsync([FromQuery] ObraSocialFiltroDto filtro)
        {
            // Caso 1: no se ingresó nada
            if (string.IsNullOrWhiteSpace(filtro.Busqueda))
            {
                return BadRequest(new { mensaje = "Debe ingresar un valor de búsqueda." });
            }
            var listaObrasSociales = await _obraSocialRepository.BuscarAsync(filtro);
            // Caso 2: se ingresó un valor, pero no hay coincidencias

            if (listaObrasSociales == null || !listaObrasSociales.Any())
            {
                return NotFound(new { mensaje = $"No se encontró ninguna obra social con el parámetro de búsqueda '{filtro.Busqueda}'." });
            }

            // Caso 3: hay resultados
            return Ok(listaObrasSociales);
        }
    }
}
