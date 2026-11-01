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
        private readonly IInformeSociosPdf _informeSociosPdf;

        public ReportesController(ISocioRepository socioRepository, IInformeSociosPdf informeSociosPdf)
        {
            _socioRepository = socioRepository;
            _informeSociosPdf = informeSociosPdf;
        }

        [HttpGet("socios")]
        public async Task<IActionResult> InformeSociosAsync([FromQuery] SocioInformeFiltroDto filtro)
        {
            var error = ValidarFiltro(filtro);
            if (error is not null)
                return BadRequest(error);

            var reporte = await _socioRepository.GenerarInformeAsync(filtro);

            if (!reporte.Any())
                return NotFound("No se encontraron socios con los criterios seleccionados.");

            return Ok(reporte);
        }

        /// <summary>
        /// Mismo informe que GET socios, pero en PDF (A4 apaisada). Recibe los mismos filtros.
        /// Se devuelve para mostrar en el navegador ("inline"), desde donde se puede imprimir o descargar.
        ///   - Estado inválido o ninguna categoría marcada → 400.
        ///   - Ningún socio con esos criterios             → 404.
        /// </summary>
        [HttpGet("socios/pdf")]
        public async Task<IActionResult> InformeSociosPdfAsync([FromQuery] SocioInformeFiltroDto filtro)
        {
            var error = ValidarFiltro(filtro);
            if (error is not null)
                return BadRequest(error);

            var reporte = (await _socioRepository.GenerarInformeAsync(filtro)).ToList();

            if (reporte.Count == 0)
                return NotFound("No se encontraron socios con los criterios seleccionados.");

            var pdf = _informeSociosPdf.Generar(reporte, filtro);

            // "inline": el navegador lo muestra en vez de forzar la descarga.
            Response.Headers.ContentDisposition = "inline; filename=\"informe-socios.pdf\"";
            return File(pdf, "application/pdf");
        }

        /// <summary>
        /// Valida el filtro y normaliza el estado (mayúsculas, sin espacios).
        /// Devuelve el mensaje de error, o null si el filtro es válido.
        /// </summary>
        private static string? ValidarFiltro(SocioInformeFiltroDto filtro)
        {
            var estado = filtro.Estado?.Trim().ToUpperInvariant();
            if (estado is not ("ACTIVO" or "INACTIVO" or "TODOS"))
                return "El estado debe ser ACTIVO, INACTIVO o TODOS.";

            if (!filtro.IncluirAdherentes && !filtro.IncluirSepelio && !filtro.IncluirNicho)
                return "Debe seleccionar al menos una categoría: adherente, sepelio o nicho.";

            filtro.Estado = estado;
            return null;
        }
    }
}
