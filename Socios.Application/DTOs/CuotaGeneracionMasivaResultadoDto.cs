namespace Socios.Application.DTOs
{
    /// <summary>
    /// Resumen de la generación masiva de un mes: socios procesados, cuotas creadas, cuotas
    /// omitidas por ya existir, y socios con errores de carga (no se les generó ninguna
    /// cuota).
    /// </summary>
    public class CuotaGeneracionMasivaResultadoDto
    {
        /// <summary>Cantidad de socios activos procesados.</summary>
        public int SociosProcesados { get; set; }

        /// <summary>Cantidad de cuotas efectivamente creadas (societarias + sepelio).</summary>
        public int CuotasGeneradas { get; set; }

        /// <summary>Cantidad de cuotas que ya existían para ese mes y se omitieron.</summary>
        public int CuotasOmitidas { get; set; }

        /// <summary>Socios a los que no se les pudo generar el sepelio, con el motivo.</summary>
        public List<SocioInconsistenciaDto> Inconsistencias { get; set; } = new();
    }
}
