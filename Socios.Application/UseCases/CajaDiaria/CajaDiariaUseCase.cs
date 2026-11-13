using System.Globalization;
using Socios.Application.DTOs;
using Socios.Application.Exceptions;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Application.UseCases.CajaDiaria
{
    /// <summary>
    /// Casos de uso de la caja diaria. La tabla estado_cajadiaria registra eventos:
    ///   - APERTURA: con cuánto efectivo empieza el usuario.
    ///   - CIERRE:   con cuánto efectivo termina.
    /// Cada apertura recibe un número de caja nuevo (correlativo); el cierre de esa caja se
    /// registra con el mismo número. Los movimientos de caja se cuelgan del registro de
    /// APERTURA (Id_CajaDiaria), que es el que lleva el número.
    /// </summary>
    public class CajaDiariaUseCase : ICajaDiariaUseCase
    {
        private const string TipoApertura = "APERTURA";
        private const string TipoCierre = "CIERRE";
        private const string TipoIngreso = "INGRESO";
        private const string TipoEgreso = "EGRESO";
        private const string CuentaEfectivo = "EFECTIVO";
        private const string CuentaActiva = "ACTIVA";
        private const string MetodoEfectivo = "EFECTIVO";
        private const string MetodoTransferenciaBancaria = "TRANSFERENCIA BANCARIA";
        private const string DetalleTransferencia = "TRANSFERENCIA";

        /// <summary>
        /// Entidad del propio Centro, a la que se imputan los movimientos internos (la crea la
        /// migración EntidadCentroMovimientosInternos). Se ubica por su razón social exacta.
        /// </summary>
        private const string RazonSocialCentro = "CENTRO DE JUBILADOS Y PENSIONADOS DE ROLDÁN";

        private const int LargoMaximoObservacion = 200;

        private static readonly CultureInfo Pesos = CultureInfo.GetCultureInfo("es-AR");

        private readonly IEstadoCajaDiariaRepository _cajasDiarias;
        private readonly ICajaRepository _caja;
        private readonly ICuentaRepository _cuentas;
        private readonly IEntidadRepository _entidades;
        private readonly IMetodoPagoRepository _metodosPago;
        private readonly IDetalleMovimientoRepository _detallesMovimiento;
        private readonly IComprobanteTransferenciaPdf _comprobanteTransferenciaPdf;
        private readonly IUnitOfWork _unitOfWork;

        public CajaDiariaUseCase(
            IEstadoCajaDiariaRepository cajasDiarias,
            ICajaRepository caja,
            ICuentaRepository cuentas,
            IEntidadRepository entidades,
            IMetodoPagoRepository metodosPago,
            IDetalleMovimientoRepository detallesMovimiento,
            IComprobanteTransferenciaPdf comprobanteTransferenciaPdf,
            IUnitOfWork unitOfWork)
        {
            _comprobanteTransferenciaPdf = comprobanteTransferenciaPdf;
            _cajasDiarias = cajasDiarias;
            _caja = caja;
            _cuentas = cuentas;
            _entidades = entidades;
            _metodosPago = metodosPago;
            _detallesMovimiento = detallesMovimiento;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CuentaListadoDto>> ListarCuentasActivasAsync()
        {
            var cuentas = await _cuentas.ListarActivasAsync();

            return cuentas
                .Select(c => new CuentaListadoDto
                {
                    IdCuenta = c.Id_Cuenta,
                    NombreCuenta = c.NombreCuenta,
                    NroCuenta = c.NroCuenta
                })
                .ToList();
        }

        public async Task<CajaAbiertaDto> ObtenerCajaAbiertaAsync(int idUsuario)
        {
            var apertura = await _cajasDiarias.ObtenerCajaAbiertaAsync(idUsuario)
                ?? throw new RecursoNoEncontradoException("No tiene una caja abierta.");

            return new CajaAbiertaDto
            {
                IdCajaDiaria = apertura.Id_CajaDiaria,
                NroCaja = apertura.Nro_Caja,
                FechaHoraApertura = apertura.FechaHora,
                SaldoInicial = apertura.Saldo ?? 0m
            };
        }

        /// <summary>
        /// Abre la caja. Reglas (decisiones del usuario 2026-11-07):
        ///   - El usuario no puede tener otra caja abierta (aunque sea de otro día): primero
        ///     tiene que cerrarla. Puede abrir varias veces en el mismo día si cerró la anterior.
        ///   - El número de caja lo asigna el sistema: el mayor existente + 1.
        ///   - El saldo inicial es el efectivo que declara el usuario; no se exige que coincida
        ///     con su último cierre, pero se informa la diferencia.
        ///   - Usuario y fecha/hora no se reciben: la caja es del usuario autenticado y se abre
        ///     con la hora del servidor (la misma que usan los cobros).
        /// Todo dentro de una transacción con las cajas bloqueadas: dos aperturas a la vez no
        /// toman el mismo número, y un doble clic no abre dos cajas.
        /// </summary>
        public async Task<CajaAperturaResultadoDto> AbrirAsync(CajaAperturaDto dto, int idUsuario)
        {
            var saldoInicial = dto.SaldoInicial!.Value;

            return await _unitOfWork.EjecutarEnTransaccionAsync(async () =>
            {
                await _cajasDiarias.BloquearCajasAsync();

                // Se verifica DESPUÉS del bloqueo: si llegan dos aperturas del mismo usuario, la
                // segunda ya ve la caja que abrió la primera.
                var cajaAbierta = await _cajasDiarias.ObtenerCajaAbiertaAsync(idUsuario);
                if (cajaAbierta is not null)
                    throw new ReglaNegocioException(
                        $"Ya tiene abierta la caja N° {cajaAbierta.Nro_Caja} desde el " +
                        $"{cajaAbierta.FechaHora:dd/MM/yyyy HH:mm}: debe cerrarla antes de abrir una nueva.");

                var ultimoCierre = await _cajasDiarias.ObtenerUltimoCierreAsync(idUsuario);
                var nroCaja = await _cajasDiarias.ObtenerSiguienteNroCajaAsync();

                var apertura = new EstadoCajaDiaria
                {
                    Nro_Caja = nroCaja,
                    Id_Usuario = idUsuario,
                    Tipo = TipoApertura,
                    FechaHora = DateTime.Now,
                    Saldo = saldoInicial
                };
                _cajasDiarias.Agregar(apertura);

                await _unitOfWork.GuardarCambiosAsync();

                return new CajaAperturaResultadoDto
                {
                    Mensaje = $"Caja N° {nroCaja} abierta correctamente.",
                    IdCajaDiaria = apertura.Id_CajaDiaria,
                    NroCaja = nroCaja,
                    FechaHoraApertura = apertura.FechaHora,
                    SaldoInicial = saldoInicial,
                    NroCajaUltimoCierre = ultimoCierre?.Nro_Caja,
                    FechaHoraUltimoCierre = ultimoCierre?.FechaHora,
                    SaldoUltimoCierre = ultimoCierre?.Saldo,
                    Diferencia = ultimoCierre?.Saldo is decimal saldoCierre ? saldoInicial - saldoCierre : null
                };
            });
        }

        /// <summary>
        /// Cierra la caja abierta del usuario. Reglas (decisiones del usuario 2026-11-07):
        ///   - Solo se recibe el efectivo contado; usuario y fecha/hora salen del login y del
        ///     servidor. El CIERRE se registra con el mismo número de caja que la APERTURA.
        ///   - El efectivo contado tiene que coincidir EXACTO con el que calcula el sistema:
        ///     saldo inicial + ingresos − egresos de esa caja en la cuenta EFECTIVO (lo que pasó
        ///     por transferencia u otra cuenta no es efectivo).
        ///   - Si no coincide, NO se cierra: se informa el esperado y la diferencia (faltante o
        ///     sobrante). El cajero tiene que registrar el movimiento que falta (ej. una
        ///     transferencia del efectivo al banco) o un ajuste de diferencia, y volver a cerrar.
        ///     Este caso de uso no genera ajustes.
        /// Con las cajas bloqueadas (exclusivo): mientras se calcula y se cierra no entra ningún
        /// cobro, así que el esperado no cambia por debajo, y un doble clic no cierra dos veces.
        /// </summary>
        public async Task<CajaCierreResultadoDto> CerrarAsync(CajaCierreDto dto, int idUsuario)
        {
            var saldoFinal = dto.SaldoFinal!.Value;

            var cuentaEfectivo = await _cuentas.ObtenerPorNombreAsync(CuentaEfectivo)
                ?? throw new ReglaNegocioException("No está configurada la cuenta EFECTIVO.");

            return await _unitOfWork.EjecutarEnTransaccionAsync(async () =>
            {
                await _cajasDiarias.BloquearCajasAsync();

                var apertura = await _cajasDiarias.ObtenerCajaAbiertaAsync(idUsuario)
                    ?? throw new ReglaNegocioException("No tiene una caja abierta para cerrar.");

                var (saldoInicial, ingresos, egresos, saldoEsperado) =
                    await CalcularEfectivoAsync(apertura, cuentaEfectivo.Id_Cuenta);

                if (saldoFinal != saldoEsperado)
                {
                    var diferencia = saldoFinal - saldoEsperado;
                    var tipoDiferencia = diferencia < 0 ? "faltante" : "sobrante";
                    throw new ReglaNegocioException(
                        $"No se puede cerrar la caja N° {apertura.Nro_Caja}: el efectivo declarado " +
                        $"({Importe(saldoFinal)}) no coincide con el del sistema ({Importe(saldoEsperado)}). " +
                        $"Hay un {tipoDiferencia} de {Importe(Math.Abs(diferencia))}. Registre el movimiento " +
                        "que falte o el ajuste de diferencia y vuelva a cerrar la caja.");
                }

                var cierre = new EstadoCajaDiaria
                {
                    Nro_Caja = apertura.Nro_Caja,
                    Id_Usuario = idUsuario,
                    Tipo = TipoCierre,
                    FechaHora = DateTime.Now,
                    Saldo = saldoFinal
                };
                _cajasDiarias.Agregar(cierre);

                await _unitOfWork.GuardarCambiosAsync();

                return new CajaCierreResultadoDto
                {
                    Mensaje = $"Caja N° {apertura.Nro_Caja} cerrada correctamente.",
                    IdCajaDiaria = cierre.Id_CajaDiaria,
                    NroCaja = apertura.Nro_Caja,
                    FechaHoraApertura = apertura.FechaHora,
                    FechaHoraCierre = cierre.FechaHora,
                    SaldoInicial = saldoInicial,
                    IngresosEfectivo = ingresos,
                    EgresosEfectivo = egresos,
                    SaldoFinal = saldoFinal
                };
            });
        }

        /// <summary>
        /// Transfiere dinero entre dos cuentas del Centro (ej. depositar el efectivo en el banco,
        /// extraer del banco al efectivo, o entre cuentas del banco). Reglas (decisiones del
        /// usuario 2026-11-09):
        ///   - Se registra en la caja abierta del usuario (sin caja abierta → 409), como dos
        ///     movimientos con detalle TRANSFERENCIA imputados a la entidad del Centro: EGRESO de
        ///     la cuenta de origen e INGRESO en la de destino, con la misma fecha/hora.
        ///   - Ambas cuentas tienen que existir, estar ACTIVAS y ser distintas.
        ///   - Si sale de EFECTIVO, el importe no puede superar el efectivo que el sistema calcula
        ///     para la caja del usuario: no se puede depositar plata que la caja no tiene (si no,
        ///     después el cierre nunca cuadraría). De los bancos no se valida saldo: el sistema no
        ///     lo conoce.
        ///   - Al registrarse en la caja de quien la hace, solo afecta el cierre de ESA caja (el
        ///     cierre cuenta los movimientos en EFECTIVO de su propia caja).
        ///   - Método de pago: EFECTIVO si la transferencia toca la cuenta EFECTIVO (depósito o
        ///     extracción); TRANSFERENCIA BANCARIA si es entre cuentas del banco.
        /// Con las cajas bloqueadas en exclusivo: el efectivo disponible no cambia mientras se
        /// valida (ni por otra transferencia por doble clic, ni por un cierre). Es una operación
        /// poco frecuente y breve, así que frenar los cobros unos milisegundos no se nota.
        /// </summary>
        public async Task<TransferenciaResultadoDto> TransferirAsync(TransferenciaCrearDto dto, int idUsuario)
        {
            var importe = dto.Importe!.Value;

            var origen = await _cuentas.ObtenerPorIdAsync(dto.IdCuentaOrigen!.Value)
                ?? throw new RecursoNoEncontradoException("No se encontró la cuenta de origen.");
            var destino = await _cuentas.ObtenerPorIdAsync(dto.IdCuentaDestino!.Value)
                ?? throw new RecursoNoEncontradoException("No se encontró la cuenta de destino.");

            if (origen.Estado != CuentaActiva)
                throw new ReglaNegocioException($"La cuenta de origen {origen.NombreCuenta} no está activa.");
            if (destino.Estado != CuentaActiva)
                throw new ReglaNegocioException($"La cuenta de destino {destino.NombreCuenta} no está activa.");

            var cuentaEfectivo = await _cuentas.ObtenerPorNombreAsync(CuentaEfectivo)
                ?? throw new ReglaNegocioException("No está configurada la cuenta EFECTIVO.");
            var centro = await _entidades.ObtenerPorRazonSocialAsync(RazonSocialCentro)
                ?? throw new ReglaNegocioException($"No está configurada la entidad '{RazonSocialCentro}'.");
            var detalle = await _detallesMovimiento.ObtenerPorNombreAsync(DetalleTransferencia)
                ?? throw new ReglaNegocioException($"No está configurado el detalle de movimiento '{DetalleTransferencia}'.");

            var saleDeEfectivo = origen.Id_Cuenta == cuentaEfectivo.Id_Cuenta;
            var tocaEfectivo = saleDeEfectivo || destino.Id_Cuenta == cuentaEfectivo.Id_Cuenta;
            var nombreMetodo = tocaEfectivo ? MetodoEfectivo : MetodoTransferenciaBancaria;
            var metodoPago = await _metodosPago.ObtenerPorNombreAsync(nombreMetodo)
                ?? throw new ReglaNegocioException($"No está configurado el método de pago '{nombreMetodo}'.");

            var observacion = ArmarObservacionTransferencia(origen, destino, dto.NroComprobante);

            return await _unitOfWork.EjecutarEnTransaccionAsync(async () =>
            {
                await _cajasDiarias.BloquearCajasAsync();

                var apertura = await _cajasDiarias.ObtenerCajaAbiertaAsync(idUsuario)
                    ?? throw new ReglaNegocioException("No tiene una caja abierta: debe abrir la caja antes de registrar una transferencia.");

                if (saleDeEfectivo)
                {
                    var (_, _, _, disponible) = await CalcularEfectivoAsync(apertura, cuentaEfectivo.Id_Cuenta);
                    if (importe > disponible)
                        throw new ReglaNegocioException(
                            $"No puede transferir {Importe(importe)} desde EFECTIVO: según el sistema, su caja " +
                            $"N° {apertura.Nro_Caja} tiene {Importe(disponible)} en efectivo.");
                }

                var fechaHora = DateTime.Now;

                var egreso = NuevoMovimiento(apertura, fechaHora, TipoEgreso, origen, importe);
                var ingreso = NuevoMovimiento(apertura, fechaHora, TipoIngreso, destino, importe);
                _caja.Agregar(egreso);
                _caja.Agregar(ingreso);

                await _unitOfWork.GuardarCambiosAsync();

                return new TransferenciaResultadoDto
                {
                    Mensaje = "La transferencia fue registrada correctamente.",
                    IdMovimientoEgreso = egreso.Id_Movimiento,
                    IdMovimientoIngreso = ingreso.Id_Movimiento,
                    NroCaja = apertura.Nro_Caja,
                    FechaHora = fechaHora,
                    CuentaOrigen = origen.NombreCuenta,
                    CuentaDestino = destino.NombreCuenta,
                    Importe = importe
                };
            });

            Caja NuevoMovimiento(EstadoCajaDiaria apertura, DateTime fechaHora, string tipo, Cuenta cuenta, decimal monto) => new()
            {
                Id_CajaDiaria = apertura.Id_CajaDiaria,
                FechaHoraMov = fechaHora,
                Tipo_Movimiento = tipo,
                Id_Entidad = centro.Id_Entidad,
                Monto = monto,
                Id_MetodoPago = metodoPago.Id_MetodoPago,
                Id_Cuenta = cuenta.Id_Cuenta,
                Id_Detmov = detalle.Id_Detmov,
                Observacion = observacion
            };
        }

        /// <summary>
        /// Genera (o reimprime) el PDF del comprobante de una transferencia. Acepta el Id de
        /// cualquiera de sus dos movimientos: siempre devuelve el mismo comprobante.
        /// </summary>
        public async Task<byte[]> GenerarComprobanteTransferenciaAsync(int idMovimiento)
        {
            var comprobante = await _caja.ObtenerComprobanteTransferenciaAsync(idMovimiento)
                ?? throw new RecursoNoEncontradoException("No se encontró la transferencia indicada.");

            return _comprobanteTransferenciaPdf.Generar(comprobante);
        }

        /// <summary>
        /// Efectivo de una caja según el sistema: saldo inicial + ingresos − egresos de esa caja
        /// en la cuenta EFECTIVO. Es el ÚNICO lugar donde vive la cuenta (la usan el cierre y la
        /// transferencia que sale de efectivo).
        /// </summary>
        private async Task<(decimal SaldoInicial, decimal Ingresos, decimal Egresos, decimal Esperado)> CalcularEfectivoAsync(
            EstadoCajaDiaria apertura, int idCuentaEfectivo)
        {
            var saldoInicial = apertura.Saldo ?? 0m;
            var (ingresos, egresos) = await _caja.ObtenerTotalesPorCuentaAsync(apertura.Id_CajaDiaria, idCuentaEfectivo);

            return (saldoInicial, ingresos, egresos, saldoInicial + ingresos - egresos);
        }

        /// <summary>"TRANSFERENCIA EFECTIVO A BANCO MACRO - COMP. 123456" (recortada al largo de la columna).</summary>
        private static string ArmarObservacionTransferencia(Cuenta origen, Cuenta destino, string? nroComprobante)
        {
            var observacion = $"TRANSFERENCIA {origen.NombreCuenta} A {destino.NombreCuenta}";

            if (!string.IsNullOrWhiteSpace(nroComprobante))
                observacion += $" - COMP. {nroComprobante.Trim().ToUpperInvariant()}";

            return observacion.Length > LargoMaximoObservacion
                ? observacion[..LargoMaximoObservacion]
                : observacion;
        }

        /// <summary>Importe en pesos con formato argentino: "$ 19.500,00".</summary>
        private static string Importe(decimal monto) => "$ " + monto.ToString("N2", Pesos);
    }
}
