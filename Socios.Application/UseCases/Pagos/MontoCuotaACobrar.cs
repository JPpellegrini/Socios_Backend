namespace Socios.Application.UseCases.Pagos
{
    /// <summary>
    /// Monto a cobrar de una cuota. Es el ÚNICO lugar donde vive la regla (la usan el cobro, la
    /// consulta previa del importe y el estado de cuenta del socio):
    ///   - PENDIENTE: el monto guardado en la cuota.
    ///   - VENCIDA: el importe vigente de su tipo de cuota en la configuración; excepto
    ///     NICHO, cuyo monto sale de la financiación del nicho y no de la configuración.
    /// </summary>
    public static class MontoCuotaACobrar
    {
        public const string EstadoVencida = "VENCIDA";
        public const string ConceptoNicho = "NICHO";

        public static decimal Calcular(string estado, string concepto, decimal montoCuota, decimal importeVigente) =>
            ActualizaAValorVigente(estado, concepto) ? importeVigente : montoCuota;

        /// <summary>Indica si la cuota se cobra al importe vigente en vez de a su monto guardado.</summary>
        public static bool ActualizaAValorVigente(string estado, string concepto) =>
            estado == EstadoVencida && concepto != ConceptoNicho;
    }
}
