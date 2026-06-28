using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/socios")]
    [Authorize]
    public class SociosController : ControllerBase
    {
        private readonly ISocioRepository _socioRepository;

        public SociosController(ISocioRepository socioRepository)
        {
            _socioRepository = socioRepository;
        }

        /// <summary>
        /// Lista los socios para la pantalla "Socios".
        /// Filtros (todos opcionales, combinados con AND):
        ///   - busqueda: coincidencia parcial por Nombre o Apellido.
        ///   - dni: coincidencia parcial por DNI.
        ///   - incluirInactivos: false (default) solo ACTIVOS; true incluye INACTIVOS.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarSociosAsync([FromQuery] SocioFiltroDto filtro)
        {
            var socios = await _socioRepository.BuscarAsync(filtro);

            return Ok(socios);
        }
    }
}
