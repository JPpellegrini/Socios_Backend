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
        private readonly IInformeMovimientosSociosPdf _informeMovimientosPdf;

        public ReportesController(
            ISocioRepository socioRepository,
            IInformeSociosPdf informeSociosPdf,
            IInformeMovimientosSociosPdf informeMovimientosPdf)
        {
            _socioRepository = socioRepository;
            _informeSociosPdf = informeSociosPdf;
            _informeMovimientosPdf = informeMovimientosPdf;
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
        /// Altas y bajas de socios en un período: detalle (una fila por socio; si se dio de baja
        /// en el período, muestra la baja con su motivo) y resumen (totales y bajas por motivo)
        /// para armar el gráfico.
        ///   - Falta alguna fecha, desde > hasta o tipo inválido → 400.
        ///   - Ningún movimiento en el período                  → 404.
        /// </summary>
        [HttpGet("socios/movimientos")]
        public async Task<IActionResult> InformeMovimientosAsync([FromQuery] SocioMovimientoFiltroDto filtro)
        {
            var error = ValidarFiltro(filtro);
            if (error is not null)
                return BadRequest(error);

            var informe = await ArmarInformeMovimientosAsync(filtro);

            if (informe.Socios.Count == 0)
                return NotFound("No se encontraron altas ni bajas con los criterios seleccionados.");

            return Ok(informe);
        }

        /// <summary>
        /// Mismo informe que GET socios/movimientos, en PDF (A4 apaisada): resumen con gráfico
        /// de barras y detalle de socios. Se devuelve "inline" para verlo en el navegador.
        /// </summary>
        [HttpGet("socios/movimientos/pdf")]
        public async Task<IActionResult> InformeMovimientosPdfAsync([FromQuery] SocioMovimientoFiltroDto filtro)
        {
            var error = ValidarFiltro(filtro);
            if (error is not null)
                return BadRequest(error);

            var informe = await ArmarInformeMovimientosAsync(filtro);

            if (informe.Socios.Count == 0)
                return NotFound("No se encontraron altas ni bajas con los criterios seleccionados.");

            var pdf = _informeMovimientosPdf.Generar(informe);

            // "inline": el navegador lo muestra en vez de forzar la descarga.
            Response.Headers.ContentDisposition = "inline; filename=\"altas-bajas-socios.pdf\"";
            return File(pdf, "application/pdf");
        }

        private async Task<SocioMovimientosInformeDto> ArmarInformeMovimientosAsync(SocioMovimientoFiltroDto filtro)
        {
            var socios = await _socioRepository.ObtenerMovimientosAsync(
                filtro.FechaDesde!.Value,
                filtro.FechaHasta!.Value,
                incluirAltas: filtro.Tipo != "BAJAS",
                incluirBajas: filtro.Tipo != "ALTAS");

            return SocioMovimientosInformeDto.Armar(filtro, socios);
        }

        /// <summary>
        /// Valida el filtro de altas/bajas y normaliza el tipo (mayúsculas, sin espacios).
        /// Las fechas obligatorias ya las valida [ApiController] (400 automático).
        /// Devuelve el mensaje de error, o null si el filtro es válido.
        /// </summary>
        private static string? ValidarFiltro(SocioMovimientoFiltroDto filtro)
        {
            if (filtro.FechaDesde!.Value.Date > filtro.FechaHasta!.Value.Date)
                return "La fecha desde no puede ser posterior a la fecha hasta.";

            var tipo = filtro.Tipo?.Trim().ToUpperInvariant();
            if (tipo is not ("TODOS" or "ALTAS" or "BAJAS"))
                return "El tipo debe ser TODOS, ALTAS o BAJAS.";

            filtro.Tipo = tipo;
            return null;
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
