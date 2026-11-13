using Socios.Application.DTOs;

namespace Socios.Application.UseCases.CajaDiaria
{
    /// <summary>
    /// Punto de entrada de los casos de uso de la caja diaria (apertura y cierre).
    /// </summary>
    public interface ICajaDiariaUseCase
    {
        /// <summary>Caja abierta del usuario (sin el efectivo esperado). 404 si no tiene.</summary>
        Task<CajaAbiertaDto> ObtenerCajaAbiertaAsync(int idUsuario);

        /// <summary>
        /// Abre una caja nueva para el usuario con el efectivo declarado. Falla si el usuario
        /// ya tiene una caja abierta.
        /// </summary>
        Task<CajaAperturaResultadoDto> AbrirAsync(CajaAperturaDto dto, int idUsuario);

        /// <summary>
        /// Cierra la caja abierta del usuario si el efectivo contado coincide con el que
        /// calcula el sistema; si no coincide, falla informando la diferencia.
        /// </summary>
        Task<CajaCierreResultadoDto> CerrarAsync(CajaCierreDto dto, int idUsuario);

        /// <summary>Cuentas activas para los desplegables de las pantallas de caja.</summary>
        Task<List<CuentaListadoDto>> ListarCuentasActivasAsync();

        /// <summary>
        /// Transfiere dinero entre dos cuentas del Centro, registrándolo en la caja abierta del
        /// usuario (EGRESO de origen + INGRESO en destino).
        /// </summary>
        Task<TransferenciaResultadoDto> TransferirAsync(TransferenciaCrearDto dto, int idUsuario);
    }
}
