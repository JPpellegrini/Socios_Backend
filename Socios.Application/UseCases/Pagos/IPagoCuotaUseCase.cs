using Socios.Application.DTOs;

namespace Socios.Application.UseCases.Pagos
{
    /// <summary>
    /// Punto de entrada de los casos de uso del cobro de cuotas.
    /// </summary>
    public interface IPagoCuotaUseCase
    {
        /// <summary>
        /// Calcula cuánto se cobraría la cuota si se paga ahora (pendiente: su monto;
        /// vencida: el valor actual de configuración). No modifica nada.
        /// </summary>
        Task<CuotaImporteACobrarDto> ObtenerImporteACobrarAsync(int idCuota);

        /// <summary>
        /// Cobra una cuota en la caja abierta del usuario: registra el ingreso en caja y deja
        /// la cuota PAGADA, en una sola transacción.
        /// </summary>
        Task<CuotaPagoResultadoDto> PagarAsync(CuotaPagarDto dto, int idUsuario);

        /// <summary>Genera (o reimprime) el PDF del comprobante de pago de un movimiento de caja.</summary>
        Task<byte[]> GenerarComprobanteAsync(int idMovimiento);
    }
}
