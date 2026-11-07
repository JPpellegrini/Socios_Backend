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
        private const string CuentaEfectivo = "EFECTIVO";

        private static readonly CultureInfo Pesos = CultureInfo.GetCultureInfo("es-AR");

        private readonly IEstadoCajaDiariaRepository _cajasDiarias;
        private readonly ICajaRepository _caja;
        private readonly ICuentaRepository _cuentas;
        private readonly IUnitOfWork _unitOfWork;

        public CajaDiariaUseCase(
            IEstadoCajaDiariaRepository cajasDiarias,
            ICajaRepository caja,
            ICuentaRepository cuentas,
            IUnitOfWork unitOfWork)
        {
            _cajasDiarias = cajasDiarias;
            _caja = caja;
            _cuentas = cuentas;
            _unitOfWork = unitOfWork;
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

                var saldoInicial = apertura.Saldo ?? 0m;
                var (ingresos, egresos) = await _caja.ObtenerTotalesPorCuentaAsync(
                    apertura.Id_CajaDiaria, cuentaEfectivo.Id_Cuenta);
                var saldoEsperado = saldoInicial + ingresos - egresos;

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

        /// <summary>Importe en pesos con formato argentino: "$ 19.500,00".</summary>
        private static string Importe(decimal monto) => "$ " + monto.ToString("N2", Pesos);
    }
}
