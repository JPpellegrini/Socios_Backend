namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos de una transferencia entre cuentas listos para imprimir su comprobante. Una
    /// transferencia son dos movimientos (EGRESO de origen + INGRESO en destino) pero tiene UN
    /// solo comprobante, numerado con el Id del EGRESO.
    /// </summary>
    public class ComprobanteTransferenciaDto
    {
        /// <summary>Número del comprobante = Id del movimiento de EGRESO.</summary>
        public int IdMovimientoEgreso { get; set; }
        public int IdMovimientoIngreso { get; set; }

        public DateTime FechaHora { get; set; }
        public decimal Importe { get; set; }
        public string MetodoPago { get; set; } = null!;

        public string CuentaOrigen { get; set; } = null!;
        public int NroCuentaOrigen { get; set; }
        public string CuentaDestino { get; set; } = null!;
        public int NroCuentaDestino { get; set; }

        /// <summary>Número de comprobante u operación del banco, si se informó.</summary>
        public string? ComprobanteBancario { get; set; }

        /// <summary>Usuario que la registró (dueño de la caja) y número de caja.</summary>
        public string Cajero { get; set; } = null!;
        public int NroCaja { get; set; }
    }
}
