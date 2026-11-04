namespace Socios.Application.DTOs
{
    /// <summary>
    /// Cuánto se le va a cobrar una cuota si se paga ahora, para mostrarlo en la pantalla de
    /// pago antes de confirmar. Usa el mismo cálculo que el cobro.
    /// </summary>
    public class CuotaImporteACobrarDto
    {
        public int IdCuota { get; set; }
        public string Estado { get; set; } = null!;
        public string Concepto { get; set; } = null!;
        public int Mes { get; set; }
        public int Anio { get; set; }

        /// <summary>Monto guardado en la cuota.</summary>
        public decimal MontoOriginal { get; set; }

        /// <summary>Monto que se cobra si se paga ahora.</summary>
        public decimal MontoACobrar { get; set; }

        /// <summary>True si la cuota está vencida y se cobra al valor actual de configuración.</summary>
        public bool ActualizadaAValorVigente { get; set; }
    }
}
