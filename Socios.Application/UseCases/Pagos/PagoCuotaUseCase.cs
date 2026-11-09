using Socios.Application.DTOs;
using Socios.Application.Exceptions;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Application.UseCases.Pagos
{
    /// <summary>
    /// Casos de uso del cobro de cuotas. Coordina QUÉ hay que hacer (reglas de negocio) y
    /// delega el acceso a datos en los repositorios y la confirmación en la unidad de trabajo.
    /// </summary>
    public class PagoCuotaUseCase : IPagoCuotaUseCase
    {
        private readonly ICuotaRepository _cuotas;
        private readonly ITipoCuotaRepository _tiposCuota;
        private readonly ICajaRepository _caja;
        private readonly IEstadoCajaDiariaRepository _cajasDiarias;
        private readonly IMetodoPagoRepository _metodosPago;
        private readonly ICuentaRepository _cuentas;
        private readonly IDetalleMovimientoRepository _detallesMovimiento;
        private readonly IComprobantePagoPdf _comprobantePdf;
        private readonly IUnitOfWork _unitOfWork;

        private const string EstadoPendiente = "PENDIENTE";
        private const string EstadoVencida = "VENCIDA";
        private const string EstadoPagado = "PAGADO";
        private const string Efectivo = "EFECTIVO";
        private const string CuentaActiva = "ACTIVA";
        private const string TipoIngreso = "INGRESO";

        private static readonly string[] Meses =
            { "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" };

        public PagoCuotaUseCase(
            ICuotaRepository cuotas,
            ITipoCuotaRepository tiposCuota,
            ICajaRepository caja,
            IEstadoCajaDiariaRepository cajasDiarias,
            IMetodoPagoRepository metodosPago,
            ICuentaRepository cuentas,
            IDetalleMovimientoRepository detallesMovimiento,
            IComprobantePagoPdf comprobantePdf,
            IUnitOfWork unitOfWork)
        {
            _cuotas = cuotas;
            _tiposCuota = tiposCuota;
            _caja = caja;
            _cajasDiarias = cajasDiarias;
            _metodosPago = metodosPago;
            _cuentas = cuentas;
            _detallesMovimiento = detallesMovimiento;
            _comprobantePdf = comprobantePdf;
            _unitOfWork = unitOfWork;
        }

        public async Task<CuotaImporteACobrarDto> ObtenerImporteACobrarAsync(int idCuota)
        {
            // Trae la cuota con su tipo (concepto e importe vigente de configuración).
            var cuota = await _cuotas.ObtenerPorIdAsync(idCuota)
                ?? throw new RecursoNoEncontradoException("No se encontró la cuota solicitada.");
            var tipoCuota = cuota.TipoCuota
                ?? throw new InvalidOperationException("La cuota no tiene tipo de cuota asociado.");

            ValidarQueSeaPagable(cuota);
            var montoACobrar = CalcularMontoACobrar(cuota, tipoCuota);

            return new CuotaImporteACobrarDto
            {
                IdCuota = cuota.Id_Deuda,
                // Igual que la grilla: una pendiente con la fecha de vencimiento pasada se muestra
                // como VENCIDA (en la base sigue PENDIENTE).
                Estado = EstaVencida(cuota) ? EstadoVencida : cuota.Estado,
                Concepto = tipoCuota.Concepto,
                Mes = cuota.Mes_Periodo,
                Anio = cuota.Anio_Periodo,
                FechaVencimiento = cuota.FechaVencimiento,
                Vencida = EstaVencida(cuota),
                MontoOriginal = cuota.Monto,
                MontoACobrar = montoACobrar,
                ActualizadaAValorVigente = montoACobrar != cuota.Monto
            };
        }

        /// <summary>
        /// Cobra una cuota. Reglas:
        ///   - El usuario tiene que tener una caja abierta (APERTURA sin CIERRE): el ingreso se
        ///     registra en esa caja.
        ///   - Método de pago EFECTIVO: la cuenta es siempre EFECTIVO. Otro método (ej.
        ///     transferencia): la cuenta es obligatoria, tiene que estar ACTIVA y no puede ser
        ///     la de EFECTIVO.
        ///   - Solo se cobran cuotas PENDIENTES (no PAGADAS ni ANULADAS).
        ///   - Monto: al día → su monto; vencida (pendiente con la fecha de vencimiento ya pasada)
        ///     → el valor actual de configuración (salvo nicho, que se cobra por su monto
        ///     financiado). El monto de la cuota se actualiza al valor cobrado, para que la cuota
        ///     y la caja digan lo mismo.
        ///   - Se registra un INGRESO en caja con el detalle "CUOTA {concepto}" y la cuota pasa a
        ///     PAGADO vinculada a ese movimiento.
        /// Todo en una sola transacción, con la fila de la cuota bloqueada: si dos personas
        /// cobran la misma cuota a la vez (o hay doble clic), solo pasa el primero.
        /// </summary>
        public async Task<CuotaPagoResultadoDto> PagarAsync(CuotaPagarDto dto, int idUsuario)
        {
            var metodoPago = await _metodosPago.ObtenerPorIdAsync(dto.IdMetodoPago!.Value)
                ?? throw new RecursoNoEncontradoException("No se encontró el método de pago indicado.");
            var cuenta = await ResolverCuentaAsync(metodoPago, dto.IdCuenta);

            return await _unitOfWork.EjecutarEnTransaccionAsync(async () =>
            {
                // Bloqueo compartido de las cajas: los cobros no se frenan entre sí, pero no
                // conviven con un cierre en curso. La caja abierta se busca DESPUÉS del bloqueo,
                // así el cobro nunca cae en una caja que se acaba de cerrar (y el cierre no
                // cuadra contra un efectivo que cambió por debajo).
                await _cajasDiarias.BloquearCajasCompartidoAsync();

                // Regla de negocio: solo se cobra con la caja abierta.
                var cajaAbierta = await _cajasDiarias.ObtenerCajaAbiertaAsync(idUsuario)
                    ?? throw new ReglaNegocioException("No tiene una caja abierta: debe abrir la caja antes de cobrar.");

                // Bloquea la fila de la cuota hasta el final de la transacción.
                var cuota = await _cuotas.ObtenerParaPagoAsync(dto.IdCuota!.Value)
                    ?? throw new RecursoNoEncontradoException("No se encontró la cuota solicitada.");

                ValidarQueSeaPagable(cuota);

                var tipoCuota = await _tiposCuota.ObtenerPorIdAsync(cuota.Id_TipoCuota)
                    ?? throw new InvalidOperationException("La cuota no tiene tipo de cuota asociado.");
                var monto = CalcularMontoACobrar(cuota, tipoCuota);

                // Cada concepto tiene su detalle de movimiento: "CUOTA SOCIO", "CUOTA SEPELIO", "CUOTA NICHO".
                var nombreDetalle = $"CUOTA {tipoCuota.Concepto}";
                var detalle = await _detallesMovimiento.ObtenerPorNombreAsync(nombreDetalle)
                    ?? throw new ReglaNegocioException($"No está configurado el detalle de movimiento '{nombreDetalle}'.");

                var movimiento = new Caja
                {
                    Id_CajaDiaria = cajaAbierta.Id_CajaDiaria,
                    FechaHoraMov = DateTime.Now,
                    Tipo_Movimiento = TipoIngreso,
                    Id_Entidad = cuota.Id_Entidad,
                    Monto = monto,
                    Id_MetodoPago = metodoPago.Id_MetodoPago,
                    Id_Cuenta = cuenta.Id_Cuenta,
                    Id_Detmov = detalle.Id_Detmov,
                    Observacion = $"PAGO CUOTA {tipoCuota.Concepto} {Meses[cuota.Mes_Periodo - 1]} {cuota.Anio_Periodo}"
                };
                _caja.Agregar(movimiento);

                // La cuota queda pagada y vinculada al movimiento (EF completa el Id al insertar).
                cuota.Caja = movimiento;
                cuota.Estado = EstadoPagado;
                cuota.Monto = monto;

                await _unitOfWork.GuardarCambiosAsync();

                return new CuotaPagoResultadoDto
                {
                    Mensaje = "La cuota fue cobrada correctamente.",
                    IdMovimiento = movimiento.Id_Movimiento,
                    IdCuota = cuota.Id_Deuda,
                    MontoCobrado = monto
                };
            });
        }

        public async Task<byte[]> GenerarComprobanteAsync(int idMovimiento)
        {
            var comprobante = await _caja.ObtenerComprobantePagoAsync(idMovimiento)
                ?? throw new RecursoNoEncontradoException("No se encontró el pago de cuota indicado.");

            return _comprobantePdf.Generar(comprobante);
        }

        /// <summary>Solo se cobran cuotas adeudadas: PENDIENTE o VENCIDA.</summary>
        private static void ValidarQueSeaPagable(Cuota cuota)
        {
            if (cuota.Estado == EstadoPagado)
                throw new ReglaNegocioException("La cuota ya está paga.");

            if (cuota.Estado != EstadoPendiente && cuota.Estado != EstadoVencida)
                throw new ReglaNegocioException($"No se puede cobrar una cuota en estado {cuota.Estado}.");
        }

        /// <summary>
        /// Monto a cobrar (al día: su monto; vencida: el importe vigente, salvo NICHO). La regla vive
        /// en <see cref="MontoCuotaACobrar"/>, compartida con el estado de cuenta y el reporte de deudas.
        /// </summary>
        private static decimal CalcularMontoACobrar(Cuota cuota, TipoCuota tipoCuota) =>
            MontoCuotaACobrar.Calcular(cuota.Estado, tipoCuota.Concepto, cuota.FechaVencimiento,
                cuota.Monto, tipoCuota.Importe, DateTime.Today);

        /// <summary>
        /// Vencida: sigue PENDIENTE y su fecha de vencimiento ya pasó (o tiene el estado VENCIDA
        /// guardado). La regla vive en <see cref="MontoCuotaACobrar.EstaVencida"/>.
        /// </summary>
        private static bool EstaVencida(Cuota cuota) =>
            MontoCuotaACobrar.EstaVencida(cuota.Estado, cuota.FechaVencimiento, DateTime.Today);

        /// <summary>
        /// Cuenta donde ingresa el dinero. Con EFECTIVO es siempre la cuenta EFECTIVO; con otro
        /// método la cuenta es obligatoria, tiene que existir, estar ACTIVA y no ser EFECTIVO.
        /// </summary>
        private async Task<Cuenta> ResolverCuentaAsync(MetodoPago metodoPago, int? idCuenta)
        {
            var cuentaEfectivo = await _cuentas.ObtenerPorNombreAsync(Efectivo)
                ?? throw new ReglaNegocioException("No está configurada la cuenta EFECTIVO.");

            if (metodoPago.NombreMetodoPago == Efectivo)
            {
                if (idCuenta.HasValue && idCuenta.Value != cuentaEfectivo.Id_Cuenta)
                    throw new ReglaNegocioException("Con pago en efectivo la cuenta debe ser EFECTIVO.");
                return cuentaEfectivo;
            }

            if (!idCuenta.HasValue)
                throw new ReglaNegocioException($"Debe indicar la cuenta para el pago con {metodoPago.NombreMetodoPago}.");

            var cuenta = await _cuentas.ObtenerPorIdAsync(idCuenta.Value)
                ?? throw new RecursoNoEncontradoException("No se encontró la cuenta indicada.");

            if (cuenta.Id_Cuenta == cuentaEfectivo.Id_Cuenta)
                throw new ReglaNegocioException($"La cuenta EFECTIVO no corresponde a un pago con {metodoPago.NombreMetodoPago}.");

            if (cuenta.Estado != CuentaActiva)
                throw new ReglaNegocioException($"La cuenta {cuenta.NombreCuenta} no está activa.");

            return cuenta;
        }
    }
}
