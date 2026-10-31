using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos de financiación de un nicho: valor, cantidad de cuotas, interés, lápida y primer
    /// vencimiento. Es la entrada de la simulación de cuotas y la base del asignar (que agrega
    /// nicho y socio), así ambos reciben y validan exactamente los mismos datos.
    /// La obligatoriedad del valor de lápida (solo si ConLapida = "SI") y que el primer
    /// vencimiento sea futuro se validan como reglas de negocio en el caso de uso.
    /// </summary>
    public class NichoFinanciacionDto
    {
        [Required(ErrorMessage = "El valor total del nicho es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El valor total del nicho no puede ser negativo.")]
        public decimal? ValorTotal { get; set; }

        [Required(ErrorMessage = "La cantidad de cuotas es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de cuotas debe ser al menos 1.")]
        public int? Cuotas { get; set; }

        [Required(ErrorMessage = "El interés por cuota es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El interés por cuota no puede ser negativo.")]
        public decimal? InteresPorCuota { get; set; }

        /// <summary>
        /// Vencimiento de la PRIMERA cuota de nicho. Las siguientes vencen un mes después
        /// cada una. El caso de uso valida que sea posterior a hoy.
        /// </summary>
        [Required(ErrorMessage = "La fecha del primer vencimiento es obligatoria.")]
        public DateTime? PrimerVencimiento { get; set; }

        /// <summary>
        /// Indica si el nicho lleva lápida. Solo admite "SI" o "NO" (no distingue mayúsculas).
        /// Si es "SI", ValorLapida es obligatorio.
        /// </summary>
        [Required(ErrorMessage = "Debe indicar si el nicho lleva lápida.")]
        [RegularExpression(@"^(?i)(si|no)$", ErrorMessage = "ConLapida solo puede ser 'SI' o 'NO'.")]
        public string ConLapida { get; set; } = null!;

        [Range(1, double.MaxValue, ErrorMessage = "El valor de la lápida no puede ser negativo.")]
        public decimal? ValorLapida { get; set; }
    }
}
