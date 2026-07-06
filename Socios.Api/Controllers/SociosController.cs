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
        public async Task<IActionResult> BuscarSociosAsync([FromBody] SocioFiltroDto filtro)
        {
            var socios = await _socioRepository.BuscarAsync(filtro);

            return Ok(socios);
        }

        /// <summary>
        /// Da de alta un socio (crea en cascada la Entidad, el Socio, su EntidadTipo
        /// tipo "Socio" con Estado ACTIVO y los Contactos).
        /// Las validaciones de los campos las aplica [ApiController] automáticamente
        /// (devuelve 400 con el detalle si el modelo es inválido).
        /// </summary>
        [HttpPost("crear")]
        public async Task<IActionResult> CrearSocioAsync([FromBody] SocioCrearDto dto)
        {
            var idSocio = await _socioRepository.CrearAsync(dto);

            return Created($"/api/v1/socios/{idSocio}", new { idSocio });
        }
    }
}
