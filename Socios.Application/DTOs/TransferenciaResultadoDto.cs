namespace Socios.Application.DTOs
{
    /// <summary>
    /// Resultado de una transferencia entre cuentas. Se registra como dos movimientos de caja
    /// (EGRESO de la cuenta de origen e INGRESO en la de destino) en la caja abierta del usuario.
    /// </summary>
    public class TransferenciaResultadoDto
    {
        public string Mensaje { get; set; } = null!;

        /// <summary>Movimiento de EGRESO en la cuenta de origen.</summary>
        public int IdMovimientoEgreso { get; set; }

        /// <summary>Movimiento de INGRESO en la cuenta de destino.</summary>
        public int IdMovimientoIngreso { get; set; }

        /// <summary>Número de la caja en la que quedó registrada.</summary>
        public int NroCaja { get; set; }

        public DateTime FechaHora { get; set; }

        public string CuentaOrigen { get; set; } = null!;

        public string CuentaDestino { get; set; } = null!;

        public decimal Importe { get; set; }
    }
}
