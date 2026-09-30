namespace Socios.Application.DTOs
{
    /// <summary>
    /// Una fila de la grilla de cuotas: cruza la cuota con el socio (Entidad) al que
    /// pertenece y con su tipo de cuota. No expone la entidad de dominio, solo lo que
    /// muestra la pantalla.
    /// </summary>
    public class CuotaListadoDto
    {
        /// <summary>Id de la cuota (clave de la fila, para acciones posteriores: pagar, anular).</summary>
        public int IdCuota { get; set; }

        /// <summary>Id del socio dueño de la cuota.</summary>
        public int IdSocio { get; set; }

        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        /// <summary>Documento del socio (DNI).</summary>
        public string? Documento { get; set; }

        /// <summary>Concepto del tipo de cuota: SOCIO, SEPELIO o NICHO.</summary>
        public string TipoCuota { get; set; } = null!;

        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; } = null!;
    }
}
