namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos de una cuota listos para imprimir su comprobante de deuda: la cuota, el socio
    /// al que pertenece y su nicho (si tiene).
    /// </summary>
    public class CuotaComprobanteDto
    {
        public int IdCuota { get; set; }
        public int IdSocio { get; set; }
        public string? Apellido { get; set; }
        public string? Nombre { get; set; }
        public string? Calle { get; set; }
        public int? Altura { get; set; }
        public string? Ciudad { get; set; }
        public string? Plan { get; set; }

        /// <summary>Concepto de la cuota: SOCIO, SEPELIO o NICHO.</summary>
        public string Concepto { get; set; } = null!;

        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; } = null!;

        /// <summary>Ubicación del nicho del socio ("sector-número"), o null si no tiene nicho.</summary>
        public string? Nicho { get; set; }
    }
}
