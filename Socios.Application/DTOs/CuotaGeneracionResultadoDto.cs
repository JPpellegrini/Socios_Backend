namespace Socios.Application.DTOs
{
    /// <summary>
    /// Resumen del resultado de generar cuotas para un socio: cuántas se crearon y
    /// cuántas se omitieron por ya existir (no se generan duplicados).
    /// </summary>
    public class CuotaGeneracionResultadoDto
    {
        /// <summary>Cantidad de cuotas efectivamente creadas.</summary>
        public int CuotasGeneradas { get; set; }

        /// <summary>Cantidad de cuotas que ya existían y se omitieron (socio + sepelio).</summary>
        public int CuotasOmitidas { get; set; }
    }
}
