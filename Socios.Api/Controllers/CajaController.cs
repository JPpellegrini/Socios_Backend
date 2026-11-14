using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Application.UseCases.CajaDiaria;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/caja")]
    [Authorize]
    public class CajaController : ControllerBase
    {
        private readonly ICajaDiariaUseCase _cajaDiariaUseCase;
        private readonly ICuentaRepository _cuentaRepository;
        private readonly ICajaRepository _cajaRepository;

        public CajaController(
            ICajaDiariaUseCase cajaDiariaUseCase,
            ICuentaRepository cuentaRepository,
            ICajaRepository cajaRepository)
        {
            _cajaDiariaUseCase = cajaDiariaUseCase;
            _cuentaRepository = cuentaRepository;
            _cajaRepository = cajaRepository;
        }

        /// <summary>
        /// Consulta de movimientos de caja para la grilla. Filtros opcionales (query string, AND):
        /// fechaDesde/fechaHasta (inclusive; sin ninguna = hoy, salvo que se pida un nroCaja: ahí
        /// se trae la caja completa aunque haya abierto otro día), idUsuario (su caja chica: lo
        /// registrado en sus cajas, incluidas sus transferencias), nroCaja (una caja puntual: lo
        /// mismo que cuenta su cierre), idCuenta, entidad (nombre, apellido, razón social o DNI),
        /// descripcion (motivo u observación) y tipo (TODOS/INGRESO/EGRESO). Sin usuario ni caja
        /// se ven los movimientos de todas las cajas. Solo lectura: va directo al repositorio.
        ///   - Tipo inválido o fecha desde posterior a hasta → 400.
        ///   - Sin coincidencias → 200 con lista vacía.
        /// </summary>
        [HttpGet("movimientos")]
        public async Task<IActionResult> BuscarMovimientosAsync([FromQuery] CajaMovimientoFiltroDto filtro)
        {
            DateTime? desde = filtro.FechaDesde?.Date;
            DateTime? hasta = filtro.FechaHasta?.Date;

            if (desde is null && hasta is null)
            {
                // Con una caja puntual (nroCaja) y sin fechas NO se filtra por fecha: la caja puede
                // estar abierta desde otro día y "Mi caja" tiene que mostrar todo lo que cuenta su
                // cierre. Sin caja ni fechas, la vista por defecto es la de hoy.
                if (!filtro.NroCaja.HasValue)
                    desde = hasta = DateTime.Today;
            }
            else
            {
                // Solo una de las fechas: la que falta toma la otra (un solo día), para no invertir el rango.
                desde ??= hasta;
                hasta ??= desde;
            }

            var movimientos = await _cajaRepository.BuscarMovimientosAsync(filtro, desde, hasta);

            return Ok(movimientos);
        }

        /// <summary>
        /// Cuánto dinero hay en cada cuenta (activas e inactivas) según el sistema:
        /// saldo inicial + ingresos − egresos de los movimientos desde la fecha del saldo inicial.
        /// Solo lectura: va directo al repositorio (sin caso de uso), como las demás búsquedas.
        /// </summary>
        [HttpGet("saldos")]
        public async Task<IActionResult> ObtenerSaldosAsync()
        {
            var saldos = await _cuentaRepository.ObtenerSaldosAsync();

            return Ok(saldos);
        }

        /// <summary>
        /// Caja abierta del usuario autenticado: número, fecha/hora de apertura y saldo inicial
        /// (para las pantallas de caja). No incluye el efectivo esperado.
        ///   - El usuario no tiene caja abierta → 404.
        /// </summary>
        [HttpGet("abierta")]
        public async Task<IActionResult> ObtenerCajaAbiertaAsync()
        {
            var caja = await _cajaDiariaUseCase.ObtenerCajaAbiertaAsync(ObtenerIdUsuario());

            return Ok(caja);
        }

        /// <summary>
        /// Abre la caja diaria del usuario autenticado con el efectivo que declara. Body:
        /// { saldoInicial }. El usuario sale del login y la fecha/hora es la del servidor
        /// (se devuelve en FechaHoraApertura). El número de caja lo asigna el sistema
        /// (correlativo) y se devuelve junto con el saldo del último cierre del usuario y la
        /// diferencia (informativa, no bloquea).
        ///   - Saldo inicial ausente, negativo o con más de 2 decimales → 400.
        ///   - El usuario ya tiene una caja abierta                     → 409.
        /// </summary>
        [HttpPost("apertura")]
        public async Task<IActionResult> AbrirAsync([FromBody] CajaAperturaDto dto)
        {
            var resultado = await _cajaDiariaUseCase.AbrirAsync(dto, ObtenerIdUsuario());

            return Ok(resultado);
        }

        /// <summary>
        /// Cierra la caja abierta del usuario autenticado. Body: { saldoFinal } (efectivo
        /// contado). Solo cierra si coincide exacto con el efectivo que calcula el sistema
        /// (saldo inicial + ingresos − egresos en la cuenta EFECTIVO de esa caja).
        ///   - Saldo final ausente, negativo o con más de 2 decimales           → 400.
        ///   - Sin caja abierta / el efectivo no coincide (informa la diferencia) → 409.
        /// </summary>
        [HttpPost("cierre")]
        public async Task<IActionResult> CerrarAsync([FromBody] CajaCierreDto dto)
        {
            var resultado = await _cajaDiariaUseCase.CerrarAsync(dto, ObtenerIdUsuario());

            return Ok(resultado);
        }

        /// <summary>
        /// Cuentas activas (id, nombre, número) para los desplegables de origen y destino de la
        /// pantalla de transferencias.
        /// </summary>
        [HttpGet("cuentas")]
        public async Task<IActionResult> ListarCuentasAsync()
        {
            var cuentas = await _cajaDiariaUseCase.ListarCuentasActivasAsync();

            return Ok(cuentas);
        }

        /// <summary>
        /// Transfiere dinero entre dos cuentas del Centro (ej. EFECTIVO → banco, banco →
        /// EFECTIVO, banco → banco). Body: { idCuentaOrigen, idCuentaDestino, importe,
        /// nroComprobante? }. Se registra en la caja abierta del usuario autenticado como dos
        /// movimientos (EGRESO de origen + INGRESO en destino) y solo afecta el cierre de esa caja.
        ///   - Datos faltantes, misma cuenta, importe 0, negativo o con más de 2 decimales → 400.
        ///   - Cuenta inexistente                                                           → 404.
        ///   - Sin caja abierta / cuenta inactiva /
        ///     sale de EFECTIVO más de lo que la caja tiene según el sistema                → 409.
        /// </summary>
        [HttpPost("transferencias")]
        public async Task<IActionResult> TransferirAsync([FromBody] TransferenciaCrearDto dto)
        {
            var resultado = await _cajaDiariaUseCase.TransferirAsync(dto, ObtenerIdUsuario());

            return Ok(resultado);
        }

        /// <summary>
        /// Genera (o reimprime) el PDF del comprobante de una transferencia. Acepta el Id de
        /// cualquiera de sus dos movimientos (IdMovimientoEgreso o IdMovimientoIngreso): una
        /// transferencia tiene un solo comprobante, numerado con el egreso. Se devuelve "inline".
        ///   - Movimiento inexistente o que no es una transferencia → 404.
        /// </summary>
        [HttpGet("transferencias/{idMovimiento:int}/comprobante")]
        public async Task<IActionResult> GenerarComprobanteTransferenciaAsync(int idMovimiento)
        {
            var pdf = await _cajaDiariaUseCase.GenerarComprobanteTransferenciaAsync(idMovimiento);

            Response.Headers.ContentDisposition = $"inline; filename=\"comprobante-transferencia-{idMovimiento}.pdf\"";
            return File(pdf, "application/pdf");
        }

        /// <summary>La caja es siempre la del usuario autenticado.</summary>
        private int ObtenerIdUsuario() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
