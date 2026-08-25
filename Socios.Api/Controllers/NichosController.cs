using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/nichos")]
    [Authorize]
    public class NichosController : ControllerBase
    {
        private readonly INichoRepository _nichoRepository;

        public NichosController(INichoRepository nichoRepository)
        {
            _nichoRepository = nichoRepository;
        }

        /// <summary>
        /// Busca nichos aplicando filtros opcionales: socio (nombre, apellido o DNI),
        /// nicho (sector o número) y estado de ocupación.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarNichoAsync([FromQuery] NichoFiltroDto filtro)
        {
            var listaNichos = await _nichoRepository.BuscarNichoAsync(filtro);

            // Una búsqueda sin coincidencias es un resultado válido: 200 con lista vacía.
            return Ok(listaNichos);
        }
    }
}
