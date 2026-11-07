namespace Socios.Application.DTOs
{
    /// <summary>
    /// Resultado del cierre de caja: la caja cerrada y cómo se compone su efectivo
    /// (saldo inicial + ingresos − egresos en EFECTIVO = saldo final).
    /// </summary>
    public class CajaCierreResultadoDto
    {
        public string Mensaje { get; set; } = null!;

        /// <summary>Id del registro de CIERRE en estado_cajadiaria.</summary>
        public int IdCajaDiaria { get; set; }

        /// <summary>Número de la caja cerrada (el mismo de su apertura).</summary>
        public int NroCaja { get; set; }

        public DateTime FechaHoraApertura { get; set; }

        public DateTime FechaHoraCierre { get; set; }

        public decimal SaldoInicial { get; set; }

        /// <summary>Ingresos de la caja en la cuenta EFECTIVO.</summary>
        public decimal IngresosEfectivo { get; set; }

        /// <summary>Egresos de la caja en la cuenta EFECTIVO.</summary>
        public decimal EgresosEfectivo { get; set; }

        /// <summary>Efectivo contado al cierre (coincide con el calculado por el sistema).</summary>
        public decimal SaldoFinal { get; set; }
    }
}
