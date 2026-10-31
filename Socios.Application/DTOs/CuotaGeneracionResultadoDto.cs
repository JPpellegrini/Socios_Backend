namespace Socios.Application.DTOs
{
    /// <summary>
    /// Resumen del resultado de generar cuotas para un socio: cuántas se crearon, cuántas
    /// se omitieron por ya existir (no se generan duplicados), y los errores de carga del
    /// socio que impidieron generar alguna cuota (por ejemplo, el sepelio).
    /// </summary>
    public class CuotaGeneracionResultadoDto
    {
        /// <summary>Cantidad de cuotas efectivamente creadas.</summary>
        public int CuotasGeneradas { get; set; }

        /// <summary>Cantidad de cuotas que ya existían y se omitieron (socio + sepelio).</summary>
        public int CuotasOmitidas { get; set; }

        /// <summary>
        /// Errores de carga del socio por los que no se generó alguna cuota, con el motivo.
        /// Vacío si se generó todo lo que correspondía.
        /// </summary>
        public List<SocioInconsistenciaDto> Inconsistencias { get; set; } = new();
    }
}
