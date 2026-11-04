using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Application.UseCases.Cuotas;
using Socios.Application.UseCases.Pagos;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/cuotas")]
    [Authorize]
    public class CuotasController : ControllerBase
    {
        private readonly ICuotaRepository _cuotaRepository;
        private readonly ICuotaUseCase _cuotaUseCase;
        private readonly IPagoCuotaUseCase _pagoCuotaUseCase;

        public CuotasController(ICuotaRepository cuotaRepository, ICuotaUseCase cuotaUseCase, IPagoCuotaUseCase pagoCuotaUseCase)
        {
            _cuotaRepository = cuotaRepository;
            _cuotaUseCase = cuotaUseCase;
            _pagoCuotaUseCase = pagoCuotaUseCase;
        }

        /// <summary>
        /// Lista las cuotas para la grilla. Filtros opcionales (query string, AND):
        ///   - busqueda: campo único de socio, matchea por Nombre, Apellido o DNI.
        ///   - tipoCuota: concepto ("socio"/"sepelio"/"nicho"); vacío o "todos" no filtra.
        ///   - estado: "pagada"/"pendiente"/"vencida"/"anulada"; vacío o "todas" no filtra.
        /// Una búsqueda sin coincidencias es un resultado válido: 200 con lista vacía.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarCuotasAsync([FromQuery] CuotaFiltroDto filtro)
        {
            var cuotas = await _cuotaRepository.BuscarAsync(filtro);

            return Ok(cuotas);
        }

        /// <summary>
        /// Genera las cuotas de un socio individual en un rango de períodos (mes/año).
        /// La societaria se genera en todo el rango; la de sepelio solo hacia adelante y si
        /// el socio tiene sepelio. No genera duplicados (las que ya existen se omiten).
        /// Con un error de carga del socio (sepelio sin plan o sin nacimiento) no se genera
        /// ninguna cuota: responde 200 con el motivo en "inconsistencias".
        ///   - Validaciones de formato                       → 400 (via [ApiController]).
        ///   - Socio inexistente                             → 404 (RecursoNoEncontradoException).
        ///   - Socio inactivo / vencimiento no futuro /
        ///     desde > hasta                                 → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("generar-individual")]
        public async Task<IActionResult> GenerarIndividualAsync([FromBody] CuotaGenerarDto dto)
        {
            var resultado = await _cuotaUseCase.GenerarIndividualAsync(dto);

            return Ok(resultado);
        }

        /// <summary>
        /// Genera las cuotas de un mes/año para todos los socios activos: la societaria y,
        /// si el socio tiene sepelio y el mes es actual o futuro, la de sepelio según su plan
        /// y edad. Las que ya existen se omiten. A los socios con errores de carga no se les
        /// genera ninguna cuota y se informan en el resultado (no cortan el proceso).
        ///   - Validaciones de formato                       → 400 (via [ApiController]).
        ///   - Vencimiento no futuro / falta config. SOCIO   → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("generar-masiva")]
        public async Task<IActionResult> GenerarMasivaAsync([FromBody] CuotaGenerarMasivaDto dto)
        {
            var resultado = await _cuotaUseCase.GenerarMasivaAsync(dto);

            return Ok(resultado);
        }

        /// <summary>
        /// Anula (lógicamente) una cuota: pasa su estado a ANULADA. No borra la fila.
        /// Si es de nicho, anula todo el lote generado en la asignación y libera el nicho.
        ///   - Validaciones de formato                       → 400 (via [ApiController]).
        ///   - Cuota inexistente                             → 404 (RecursoNoEncontradoException).
        ///   - Cuota paga / ya anulada / fuera de la hora /
        ///     nicho del socio no identificable              → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("anular")]
        public async Task<IActionResult> AnularAsync([FromBody] CuotaAnularDto dto)
        {
            var resultado = await _cuotaUseCase.AnularAsync(dto);

            return Ok(resultado);
        }

        /// <summary>
        /// Genera el PDF de comprobantes de deuda de las cuotas indicadas (uno por cuota, 6 por
        /// hoja A4 con líneas de corte). Se devuelve para mostrar en el navegador ("inline"),
        /// desde donde se puede imprimir o descargar.
        ///   - Validaciones de formato (lista vacía)       → 400 (via [ApiController]).
        ///   - Alguna cuota inexistente                    → 404 (RecursoNoEncontradoException).
        ///   - Alguna cuota paga o anulada                 → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("comprobantes")]
        public async Task<IActionResult> GenerarComprobantesAsync([FromBody] CuotaComprobanteSolicitudDto dto)
        {
            var pdf = await _cuotaUseCase.GenerarComprobantesAsync(dto);

            // "inline": el navegador lo muestra en vez de forzar la descarga.
            Response.Headers.ContentDisposition = "inline; filename=\"comprobantes-cuotas.pdf\"";
            return File(pdf, "application/pdf");
        }

        /// <summary>
        /// Devuelve cuánto se cobraría la cuota si se paga ahora, para mostrarlo en la pantalla
        /// de pago antes de confirmar (al día: su monto; vencida, es decir pendiente con la fecha
        /// de vencimiento ya pasada: el valor actual de configuración, salvo nicho). Usa el mismo
        /// cálculo que el cobro. No modifica nada.
        ///   - Cuota inexistente      → 404 (RecursoNoEncontradoException).
        ///   - Cuota paga o anulada   → 409 (ReglaNegocioException).
        /// </summary>
        [HttpGet("{idCuota:int}/importe-a-cobrar")]
        public async Task<IActionResult> ObtenerImporteACobrarAsync(int idCuota)
        {
            var importe = await _pagoCuotaUseCase.ObtenerImporteACobrarAsync(idCuota);

            return Ok(importe);
        }

        /// <summary>
        /// Cobra una cuota en la caja abierta del usuario logueado: registra el ingreso en caja
        /// y deja la cuota PAGADA. El monto lo calcula el backend. Devuelve el IdMovimiento,
        /// con el que se pide el comprobante de pago.
        ///   - Validaciones de formato                          → 400 (via [ApiController]).
        ///   - Cuota, método de pago o cuenta inexistentes      → 404 (RecursoNoEncontradoException).
        ///   - Sin caja abierta / cuota paga o anulada /
        ///     cuenta inválida para el método de pago           → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("pagar")]
        public async Task<IActionResult> PagarAsync([FromBody] CuotaPagarDto dto)
        {
            // El usuario sale de la autenticación: el cobro se registra en SU caja abierta.
            var idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var resultado = await _pagoCuotaUseCase.PagarAsync(dto, idUsuario);

            return Ok(resultado);
        }

        /// <summary>
        /// Genera (o reimprime) el PDF del comprobante de pago de una cuota, NO válido como
        /// factura, a partir del IdMovimiento devuelto al cobrar. Se devuelve para mostrar en
        /// el navegador ("inline"), desde donde se puede imprimir o descargar.
        ///   - Movimiento inexistente o que no es un pago de cuota → 404.
        /// </summary>
        [HttpGet("pagos/{idMovimiento:int}/comprobante")]
        public async Task<IActionResult> GenerarComprobantePagoAsync(int idMovimiento)
        {
            var pdf = await _pagoCuotaUseCase.GenerarComprobanteAsync(idMovimiento);

            Response.Headers.ContentDisposition = $"inline; filename=\"comprobante-pago-{idMovimiento}.pdf\"";
            return File(pdf, "application/pdf");
        }
    }
}
