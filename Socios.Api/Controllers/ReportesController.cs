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
        private readonly IMovimientosEntidadRepository _movimientosEntidadRepository;
        private readonly IInformeMovimientosEntidadPdf _informeMovimientosEntidadPdf;
        private readonly ISocioEstadoCuentaRepository _socioEstadoCuentaRepository;
        private readonly IEstadoCuentaSocioPdf _estadoCuentaSocioPdf;
        private readonly IMovimientosCajaRepository _movimientosCajaRepository;
        private readonly IInformeMovimientosCajaPdf _informeMovimientosCajaPdf;

        /// <summary>Tipos de actividad (detalles de movimiento) para el desplegable del reporte de ingresos y egresos.</summary>
        [HttpGet("caja/actividades")]
        public async Task<IActionResult> ListarActividadesCajaAsync()
        {
            return Ok(await _movimientosCajaRepository.ListarActividadesAsync());
        }

        /// <summary>
        /// Ingresos y egresos de la institución en un período (sirve para el reporte mensual o
        /// anual según el rango): detalle de movimientos, totales y subtotales por actividad y por mes.
        ///   - Falta una fecha, desde > hasta, tipo inválido o actividad inexistente → 400.
        ///   - Sin movimientos → 200 con la lista vacía y totales en 0.
        /// </summary>
        [HttpGet("caja/movimientos")]
        public async Task<IActionResult> MovimientosCajaAsync([FromQuery] MovimientosCajaFiltroDto filtro)
        {
            var (informe, error) = await ArmarMovimientosCajaAsync(filtro);
            if (informe is null)
                return error!;

            return Ok(new
            {
                informe.FechaDesde,
                informe.FechaHasta,
                informe.TipoMovimiento,
                informe.Actividad,
                informe.Cantidad,
                informe.TotalIngresos,
                informe.TotalEgresos,
                informe.Saldo,
                informe.PorActividad,
                informe.PorMes,
                informe.Movimientos
            });
        }

        /// <summary>
        /// Mismo reporte que GET caja/movimientos, en PDF (A4 apaisada).
        /// Se devuelve "inline" para verlo en el navegador.
        /// </summary>
        [HttpGet("caja/movimientos/pdf")]
        public async Task<IActionResult> MovimientosCajaPdfAsync([FromQuery] MovimientosCajaFiltroDto filtro)
        {
            var (informe, error) = await ArmarMovimientosCajaAsync(filtro);
            if (informe is null)
                return error!;

            var pdf = _informeMovimientosCajaPdf.Generar(informe);

            // "inline": el navegador lo muestra en vez de forzar la descarga.
            var nombre = $"ingresos-egresos-{informe.FechaDesde:yyyyMMdd}-{informe.FechaHasta:yyyyMMdd}.pdf";
            Response.Headers.ContentDisposition = $"inline; filename=\"{nombre}\"";
            return File(pdf, "application/pdf");
        }

        /// <summary>
        /// Valida el filtro y arma el reporte de ingresos y egresos. Si algo falla, devuelve null y
        /// la respuesta de error. Las fechas obligatorias ya las valida [ApiController].
        /// </summary>
        private async Task<(MovimientosCajaInformeDto? Informe, IActionResult? Error)> ArmarMovimientosCajaAsync(
            MovimientosCajaFiltroDto filtro)
        {
            var desde = filtro.FechaDesde!.Value.Date;
            var hasta = filtro.FechaHasta!.Value.Date;
            if (desde > hasta)
                return (null, BadRequest("La fecha desde no puede ser posterior a la fecha hasta."));

            var tipo = string.IsNullOrWhiteSpace(filtro.TipoMovimiento)
                ? MovimientosCajaFiltroDto.TodosLosTipos
                : filtro.TipoMovimiento.Trim().ToUpperInvariant();

            string? tipoMovimiento = tipo switch
            {
                MovimientosCajaFiltroDto.TodosLosTipos => null,
                MovimientosCajaFiltroDto.Ingresos => MovimientosCajaInformeDto.TipoIngreso,
                MovimientosCajaFiltroDto.Egresos => MovimientosCajaInformeDto.TipoEgreso,
                _ => "INVALIDO"
            };
            if (tipoMovimiento == "INVALIDO")
                return (null, BadRequest("El tipo de movimiento debe ser TODOS, INGRESOS o EGRESOS."));

            string? actividad = null;
            if (filtro.IdActividad.HasValue)
            {
                actividad = await _movimientosCajaRepository.ObtenerNombreActividadAsync(filtro.IdActividad.Value);
                if (actividad is null)
                    return (null, BadRequest($"No existe la actividad {filtro.IdActividad.Value}."));
            }

            var movimientos = await _movimientosCajaRepository.ObtenerMovimientosAsync(
                desde, hasta, tipoMovimiento, filtro.IdActividad);

            return (new MovimientosCajaInformeDto
            {
                FechaDesde = desde,
                FechaHasta = hasta,
                TipoMovimiento = tipo,
                Actividad = actividad,
                Movimientos = movimientos
            }, null);
        }

        public ReportesController(
            ISocioRepository socioRepository,
            IInformeSociosPdf informeSociosPdf,
            IInformeMovimientosSociosPdf informeMovimientosPdf,
            IMovimientosEntidadRepository movimientosEntidadRepository,
            IInformeMovimientosEntidadPdf informeMovimientosEntidadPdf,
            ISocioEstadoCuentaRepository socioEstadoCuentaRepository,
            IEstadoCuentaSocioPdf estadoCuentaSocioPdf,
            IMovimientosCajaRepository movimientosCajaRepository,
            IInformeMovimientosCajaPdf informeMovimientosCajaPdf)
        {
            _movimientosCajaRepository = movimientosCajaRepository;
            _informeMovimientosCajaPdf = informeMovimientosCajaPdf;
            _socioRepository = socioRepository;
            _informeSociosPdf = informeSociosPdf;
            _informeMovimientosPdf = informeMovimientosPdf;
            _movimientosEntidadRepository = movimientosEntidadRepository;
            _informeMovimientosEntidadPdf = informeMovimientosEntidadPdf;
            _socioEstadoCuentaRepository = socioEstadoCuentaRepository;
            _estadoCuentaSocioPdf = estadoCuentaSocioPdf;
        }

        /// <summary>
        /// Socios (activos e inactivos) para el selector del estado de cuenta.
        /// 'busqueda' (opcional) filtra por nombre, apellido o DNI. Sin coincidencias → 200 con lista vacía.
        /// </summary>
        [HttpGet("socios/seleccion")]
        public async Task<IActionResult> ListarSociosParaEstadoCuentaAsync([FromQuery] string? busqueda)
        {
            return Ok(await _socioEstadoCuentaRepository.ListarSociosAsync(busqueda));
        }

        /// <summary>
        /// Estado de cuenta individual del socio: cuotas (excepto anuladas) con vencimiento en el
        /// período, con los datos del pago si están pagas; total, pagado, adeudado y deuda total
        /// del socio a la fecha (esta última sin importar el período).
        ///   - Falta una fecha o desde > hasta → 400.
        ///   - La entidad no es socio          → 404.
        ///   - Sin cuotas en el período        → 200 con la lista vacía (la deuda total igual se informa).
        /// </summary>
        [HttpGet("socios/{idEntidad:int}/cuenta")]
        public async Task<IActionResult> EstadoCuentaSocioAsync(int idEntidad, [FromQuery] SocioEstadoCuentaFiltroDto filtro)
        {
            var (estadoCuenta, error) = await ArmarEstadoCuentaSocioAsync(idEntidad, filtro);
            if (estadoCuenta is null)
                return error!;

            return Ok(estadoCuenta);
        }

        /// <summary>
        /// Mismo reporte que GET socios/{idEntidad}/cuenta, en PDF (A4 apaisada).
        /// Se devuelve "inline" para verlo en el navegador.
        /// </summary>
        [HttpGet("socios/{idEntidad:int}/cuenta/pdf")]
        public async Task<IActionResult> EstadoCuentaSocioPdfAsync(int idEntidad, [FromQuery] SocioEstadoCuentaFiltroDto filtro)
        {
            var (estadoCuenta, error) = await ArmarEstadoCuentaSocioAsync(idEntidad, filtro);
            if (estadoCuenta is null)
                return error!;

            var pdf = _estadoCuentaSocioPdf.Generar(estadoCuenta);

            // "inline": el navegador lo muestra en vez de forzar la descarga.
            Response.Headers.ContentDisposition = $"inline; filename=\"estado-cuenta-socio-{estadoCuenta.Socio.IdSocio}.pdf\"";
            return File(pdf, "application/pdf");
        }

        /// <summary>
        /// Valida el filtro y arma el estado de cuenta. Si algo falla, devuelve null y la respuesta
        /// de error. Las fechas obligatorias ya las valida [ApiController] (400 automático).
        /// </summary>
        private async Task<(SocioEstadoCuentaDto? EstadoCuenta, IActionResult? Error)> ArmarEstadoCuentaSocioAsync(
            int idEntidad, SocioEstadoCuentaFiltroDto filtro)
        {
            var desde = filtro.FechaDesde!.Value.Date;
            var hasta = filtro.FechaHasta!.Value.Date;
            if (desde > hasta)
                return (null, BadRequest("La fecha desde no puede ser posterior a la fecha hasta."));

            var socio = await _socioEstadoCuentaRepository.ObtenerSocioAsync(idEntidad);
            if (socio is null)
                return (null, NotFound($"La entidad {idEntidad} no está registrada como socio."));

            var cuotas = await _socioEstadoCuentaRepository.ObtenerCuotasAsync(idEntidad, desde, hasta);
            var adeudadas = await _socioEstadoCuentaRepository.ObtenerCuotasAdeudadasAsync(idEntidad);
            SocioEstadoCuentaDto.CalcularMontos(cuotas);
            SocioEstadoCuentaDto.CalcularMontos(adeudadas);

            return (new SocioEstadoCuentaDto
            {
                Socio = socio,
                FechaDesde = desde,
                FechaHasta = hasta,
                Cuotas = cuotas,
                DeudaTotal = adeudadas.Sum(c => c.Monto),
                CuotasAdeudadasTotal = adeudadas.Count
            }, null);
        }

        /// <summary>
        /// Listado de entidades de un tipo (PROVEEDOR o COLABORADOR), activas e inactivas, para el
        /// selector del reporte de movimientos por entidad.
        ///   - Tipo inválido            → 400.
        ///   - Ninguna entidad del tipo → 404.
        /// </summary>
        [HttpGet("entidades")]
        public async Task<IActionResult> ListarEntidadesAsync([FromQuery] string? tipo)
        {
            var configuracion = TipoEntidadMovimientos.Obtener(tipo);
            if (configuracion is null)
                return BadRequest(MensajeTipoInvalido);

            var entidades = await _movimientosEntidadRepository.ListarEntidadesAsync(configuracion);

            if (entidades.Count == 0)
                return NotFound($"No hay entidades registradas como {configuracion.Descripcion.ToLower()}.");

            return Ok(entidades);
        }

        /// <summary>
        /// Movimientos de caja de un proveedor o colaborador en un período. La columna de importe es
        /// "Ingreso" para colaboradores y "Egreso" para proveedores; un movimiento en sentido
        /// contrario (ej.: devolución) va en negativo y resta del total.
        ///   - Tipo inválido, falta una fecha o desde > hasta → 400.
        ///   - La entidad no es del tipo indicado             → 404.
        ///   - Sin movimientos en el período                  → 200 con la lista vacía y total 0.
        /// </summary>
        [HttpGet("entidades/{idEntidad:int}/movimientos")]
        public async Task<IActionResult> MovimientosEntidadAsync(int idEntidad, [FromQuery] MovimientoEntidadFiltroDto filtro)
        {
            var (informe, resultadoError) = await ArmarMovimientosEntidadAsync(idEntidad, filtro);
            if (informe is null)
                return resultadoError!;

            return Ok(informe);
        }

        /// <summary>
        /// Mismo reporte que GET entidades/{idEntidad}/movimientos, en PDF (A4 apaisada), con el
        /// total de la columna importe. Si el período no tiene movimientos, el PDF lo indica.
        /// Se devuelve "inline" para verlo en el navegador.
        /// </summary>
        [HttpGet("entidades/{idEntidad:int}/movimientos/pdf")]
        public async Task<IActionResult> MovimientosEntidadPdfAsync(int idEntidad, [FromQuery] MovimientoEntidadFiltroDto filtro)
        {
            var (informe, resultadoError) = await ArmarMovimientosEntidadAsync(idEntidad, filtro);
            if (informe is null)
                return resultadoError!;

            var pdf = _informeMovimientosEntidadPdf.Generar(informe);

            // "inline": el navegador lo muestra en vez de forzar la descarga.
            var nombreArchivo = $"movimientos-{informe.Tipo.ToLower()}-{idEntidad}.pdf";
            Response.Headers.ContentDisposition = $"inline; filename=\"{nombreArchivo}\"";
            return File(pdf, "application/pdf");
        }

        private const string MensajeTipoInvalido = "El tipo debe ser PROVEEDOR o COLABORADOR.";

        /// <summary>
        /// Valida el filtro y arma el informe. Si algo falla, devuelve null y la respuesta de error.
        /// Las fechas y el tipo obligatorios ya los valida [ApiController] (400 automático).
        /// </summary>
        private async Task<(MovimientosEntidadInformeDto? Informe, IActionResult? Error)> ArmarMovimientosEntidadAsync(
            int idEntidad, MovimientoEntidadFiltroDto filtro)
        {
            var tipo = TipoEntidadMovimientos.Obtener(filtro.Tipo);
            if (tipo is null)
                return (null, BadRequest(MensajeTipoInvalido));

            var desde = filtro.FechaDesde!.Value.Date;
            var hasta = filtro.FechaHasta!.Value.Date;
            if (desde > hasta)
                return (null, BadRequest("La fecha desde no puede ser posterior a la fecha hasta."));

            var entidad = await _movimientosEntidadRepository.ObtenerEntidadAsync(idEntidad, tipo);
            if (entidad is null)
                return (null, NotFound($"La entidad {idEntidad} no está registrada como {tipo.Descripcion.ToLower()}."));

            var movimientos = await _movimientosEntidadRepository.ObtenerMovimientosAsync(idEntidad, tipo, desde, hasta);

            return (MovimientosEntidadInformeDto.Armar(tipo, desde, hasta, entidad, movimientos), null);
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
