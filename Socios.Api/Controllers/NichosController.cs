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
        /// Busca nicho según el criterio especificado (Nombre y Apellido del Socio).
        /// </summary>
        
        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarNichoAsync([FromQuery] NichoFiltroDto filtro)
        {
            var nicho = await _nichoRepository.BuscarNichoAsync(filtro);

            if (nicho == null)
                return NotFound("Nicho no encontrado.");

            return Ok(nicho);
        }
    }
}
