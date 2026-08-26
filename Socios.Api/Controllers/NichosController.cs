using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Application.UseCases.Nichos;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/nichos")]
    [Authorize]
    public class NichosController : ControllerBase
    {
        private readonly INichoRepository _nichoRepository;
        private readonly INichoUseCase _nichoUseCase;

        public NichosController(INichoRepository nichoRepository, INichoUseCase nichoUseCase)
        {
            _nichoRepository = nichoRepository;
            _nichoUseCase = nichoUseCase;
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

        /// <summary>
        /// Da de alta un nicho a partir de su sector y número. El nicho nace libre y sin
        /// lápida; el valor se completa después, al asignarlo a un socio.
        ///   - Validaciones de formato        → 400 (via [ApiController]).
        ///   - Sector + número ya existente    → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("crear")]
        public async Task<IActionResult> CrearNichoAsync([FromBody] NichoCrearDto dto)
        {
            var idNicho = await _nichoUseCase.CrearAsync(dto);

            return Ok(new { idNicho });
        }
    }
}
