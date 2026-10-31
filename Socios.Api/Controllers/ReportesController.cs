using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/reportes")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly ISocioRepository _socioRepository;

        public ReportesController(ISocioRepository socioRepository)
        {
            _socioRepository = socioRepository;
        }

        [HttpGet("socios")]
        public async Task<IActionResult> InformeSociosAsync([FromQuery] SocioInformeFiltroDto filtro)
        {
            var estado = filtro.Estado?.Trim().ToUpperInvariant();
            if (estado is not ("ACTIVO" or "INACTIVO" or "TODOS"))
                return BadRequest("El estado debe ser ACTIVO, INACTIVO o TODOS.");

            if (!filtro.IncluirAdherentes && !filtro.IncluirSepelio && !filtro.IncluirNicho)
                return BadRequest("Debe seleccionar al menos una categoría: adherente, sepelio o nicho.");

            filtro.Estado = estado;

            var reporte = await _socioRepository.GenerarInformeAsync(filtro);

            if (!reporte.Any())
                return NotFound("No se encontraron socios con los criterios seleccionados.");

            return Ok(reporte);
        }
    }
}

