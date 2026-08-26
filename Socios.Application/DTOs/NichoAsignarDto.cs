using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para asignar un nicho existente a un socio. Al asignarlo se cargan el valor,
    /// la financiación y la lápida, y el nicho pasa a estar ocupado.
    /// La obligatoriedad del valor de lápida (solo si ConLapida = true) se valida como
    /// regla de negocio en el caso de uso.
    /// </summary>
    public class NichoAsignarDto
    {
        [Required(ErrorMessage = "El id del nicho es obligatorio.")]
        public int? IdNicho { get; set; }

        [Required(ErrorMessage = "El id de la entidad del socio es obligatorio.")]
        public int? IdEntidad { get; set; }

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
