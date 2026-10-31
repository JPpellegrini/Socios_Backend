namespace Socios.Application.DTOs
{
    /// <summary>
    /// Resultado de simular las cuotas de un nicho: el detalle de cada cuota y los totales,
    /// para mostrarle al cliente qué va a pagar antes de asignar. Es el mismo cálculo que
    /// usa el asignar para generar las cuotas reales.
    /// </summary>
    public class NichoSimulacionCuotasDto
    {
        /// <summary>Valor del nicho + valor de la lápida (si lleva).</summary>
        public decimal TotalAFinanciar { get; set; }

        /// <summary>Porcentaje de interés que se aplica a cada cuota.</summary>
        public decimal InteresPorCuota { get; set; }

        public int CantidadCuotas { get; set; }

        /// <summary>Monto de cada cuota (todas iguales), ya con interés y redondeado.</summary>
        public decimal MontoCuota { get; set; }

        /// <summary>Lo que el cliente paga en total: la suma de todas las cuotas.</summary>
        public decimal TotalAPagar { get; set; }

        public List<NichoCuotaSimuladaDto> Cuotas { get; set; } = new();
    }
}
