namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos de un cobro de cuota listos para imprimir su comprobante de pago: el movimiento
    /// de caja, la cuota pagada, el socio y quién cobró.
    /// </summary>
    public class ComprobantePagoDto
    {
        public int IdMovimiento { get; set; }
        public DateTime FechaHoraPago { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = null!;
        public string Cuenta { get; set; } = null!;

        public int IdCuota { get; set; }
        public string Concepto { get; set; } = null!;
        public int Mes { get; set; }
        public int Anio { get; set; }

        public int IdSocio { get; set; }
        public string? Dni { get; set; }
        public string? Apellido { get; set; }
        public string? Nombre { get; set; }
        public string? Calle { get; set; }
        public int? Altura { get; set; }
        public string? Ciudad { get; set; }

        /// <summary>Usuario que cobró (dueño de la caja diaria) y número de caja.</summary>
        public string Cajero { get; set; } = null!;
        public int NroCaja { get; set; }
    }
}
