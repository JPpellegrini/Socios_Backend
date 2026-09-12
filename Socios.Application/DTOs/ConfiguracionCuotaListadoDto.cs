namespace Socios.Application.DTOs
{
    /// <summary>
    /// Una fila de la grilla de configuración de cuotas: combina el tipo de cuota
    /// (concepto, importe) con su plan asociado (A/B) y arma la columna "Edad" lista
    /// para mostrar. Los tipos de cuota sin plan (SOCIO, NICHO) muestran Plan = "-"
    /// y Edad = "SIN TOPE".
    /// </summary>
    public class ConfiguracionCuotaListadoDto
    {
        public int Id_TipoCuota { get; set; }
        public string Concepto { get; set; } = null!;
        public decimal Importe { get; set; }

        /// <summary>Texto listo para mostrar: "SIN TOPE", "HASTA 65" o "MAS DE 65".</summary>
        public string Edad { get; set; } = null!;

        /// <summary>Plan asociado ("A"/"B") o "-" si el tipo de cuota no tiene plan.</summary>
        public string Plan { get; set; } = null!;

        public DateTime UltimaModificacion { get; set; }
    }
}
