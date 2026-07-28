using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using static Socios.Application.DTOs.CiudadFiltroDto;


namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/ciudades")]
    [Authorize]
    public class CiudadesController : ControllerBase
    {
        private readonly ICiudadRepository _ciudadRepository;

        public CiudadesController(ICiudadRepository ciudadRepository)
        {
            _ciudadRepository = ciudadRepository;
        }
        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarCiudadesAsync([FromQuery] CiudadFiltroDto filtro)
        {
            var ciudad = await _ciudadRepository.BuscarAsync(filtro);

            if (ciudad == null || !ciudad.Any())
            {
                return NotFound(new { mensaje = "Ciudad no encontrada." });
            }

            return Ok(ciudad);
        }
    }
}
