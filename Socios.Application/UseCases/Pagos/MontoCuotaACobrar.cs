namespace Socios.Application.UseCases.Pagos
{
    /// <summary>
    /// Reglas de vencimiento y monto a cobrar de una cuota. Es el ÚNICO lugar donde viven: las usan
    /// el cobro y la consulta previa del importe (PagoCuotaUseCase), el estado de cuenta del socio y
    /// el reporte de deudas, para que todos calculen lo mismo.
    ///
    /// Vencimiento: una cuota está vencida si sigue PENDIENTE y su fecha de vencimiento ya pasó (la
    /// que vence hoy todavía no está vencida). El estado VENCIDA no lo asigna ningún proceso por
    /// ahora (decisión del usuario): el vencimiento sale de la fecha. Si alguna cuota tuviera el
    /// estado VENCIDA guardado, también se considera vencida.
    /// El filtro de estado de CuotaRepository.BuscarAsync aplica esta misma regla en la consulta
    /// (no puede llamar a este método desde LINQ): si cambia acá, cambiarlo también allá.
    ///
    /// Monto a cobrar:
    ///   - Al día (no vencida): el monto guardado en la cuota.
    ///   - Vencida: el importe vigente de su tipo de cuota en la configuración; excepto NICHO,
    ///     cuyo monto sale de la financiación del nicho y no de la configuración.
    /// </summary>
    public static class MontoCuotaACobrar
    {
        public const string EstadoPendiente = "PENDIENTE";
        public const string EstadoVencida = "VENCIDA";
        public const string ConceptoNicho = "NICHO";

        /// <summary>Indica si la cuota está vencida a la fecha 'hoy' (ver la regla en el resumen de la clase).</summary>
        public static bool EstaVencida(string estado, DateTime fechaVencimiento, DateTime hoy) =>
            estado == EstadoVencida
            || (estado == EstadoPendiente && fechaVencimiento.Date < hoy.Date);

        /// <summary>
        /// Estado a mostrar: VENCIDA si la cuota está vencida aunque siga guardada como PENDIENTE;
        /// en cualquier otro caso, el estado guardado.
        /// </summary>
        public static string EstadoEfectivo(string estado, DateTime fechaVencimiento, DateTime hoy) =>
            EstaVencida(estado, fechaVencimiento, hoy) ? EstadoVencida : estado;

        /// <summary>Indica si la cuota se cobra al importe vigente en vez de a su monto guardado.</summary>
        public static bool ActualizaAValorVigente(string estado, string concepto, DateTime fechaVencimiento, DateTime hoy) =>
            EstaVencida(estado, fechaVencimiento, hoy) && concepto != ConceptoNicho;

        /// <summary>Monto que se cobra a la fecha 'hoy'.</summary>
        public static decimal Calcular(string estado, string concepto, DateTime fechaVencimiento,
            decimal montoCuota, decimal importeVigente, DateTime hoy) =>
            ActualizaAValorVigente(estado, concepto, fechaVencimiento, hoy) ? importeVigente : montoCuota;
    }
}
