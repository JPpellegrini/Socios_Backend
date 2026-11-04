namespace Socios.Application.DTOs
{
    /// <summary>
    /// Resultado del cobro de una cuota. Con IdMovimiento se pide (o reimprime) el
    /// comprobante de pago.
    /// </summary>
    public class CuotaPagoResultadoDto
    {
        public string Mensaje { get; set; } = null!;

        /// <summary>Id del movimiento de caja generado (número del comprobante de pago).</summary>
        public int IdMovimiento { get; set; }

        public int IdCuota { get; set; }

        /// <summary>Monto efectivamente cobrado.</summary>
        public decimal MontoCobrado { get; set; }
    }
}
