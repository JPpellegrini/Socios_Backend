using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/buscarentidad")]
    [Authorize]
    public class EntidadesController : ControllerBase
    {
        private readonly IEntidadRepository _entidadRepository;

        public EntidadesController(IEntidadRepository entidadRepository)
        {
            _entidadRepository = entidadRepository;
        }

        /// <summary>
        /// Busca entidad según el criterio especificado (DNI).
        /// Filtros (todos opcionales):
        ///   - busqueda: coincidencia exacta por dni
        /// </summary>
        
        [HttpGet]
        public async Task<IActionResult> BuscarEntidadAsync([FromBody] EntidadFiltroDto filtro)
        {
            var entidad = await _entidadRepository.BuscarAsync(filtro);

            if (entidad == null)
                return NotFound("Entidad no encontrada.");

            return Ok(entidad);
        }
    }
}
