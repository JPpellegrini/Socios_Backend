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

        private readonly IEstadoCajaDiariaRepository _cajasDiarias;
        private readonly IUnitOfWork _unitOfWork;

        public CajaDiariaUseCase(IEstadoCajaDiariaRepository cajasDiarias, IUnitOfWork unitOfWork)
        {
            _cajasDiarias = cajasDiarias;
            _unitOfWork = unitOfWork;
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
        /// Todo dentro de una transacción con la numeración bloqueada: dos aperturas a la vez
        /// no toman el mismo número, y un doble clic no abre dos cajas.
        /// </summary>
        public async Task<CajaAperturaResultadoDto> AbrirAsync(CajaAperturaDto dto, int idUsuario)
        {
            var saldoInicial = dto.SaldoInicial!.Value;

            return await _unitOfWork.EjecutarEnTransaccionAsync(async () =>
            {
                await _cajasDiarias.BloquearNumeracionAsync();

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
    }
}
