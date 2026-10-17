using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para generar las cuotas de un socio individual en un rango de períodos
    /// (mes/año desde → mes/año hasta). El día no se informa: cada cuota cubre el mes
    /// completo. El vencimiento se recibe del cliente y se aplica a todas las cuotas
    /// generadas; el caso de uso valida que sea posterior a hoy.
    /// </summary>
    public class CuotaGenerarDto
    {
        [Required(ErrorMessage = "El id del socio es obligatorio.")]
        public int? IdSocio { get; set; }

        [Required(ErrorMessage = "El mes desde es obligatorio.")]
        [Range(1, 12, ErrorMessage = "El mes desde debe estar entre 1 y 12.")]
        public int? MesDesde { get; set; }

        [Required(ErrorMessage = "El año desde es obligatorio.")]
        [Range(2000, 2100, ErrorMessage = "El año desde no es válido.")]
        public int? AnioDesde { get; set; }

        [Required(ErrorMessage = "El mes hasta es obligatorio.")]
        [Range(1, 12, ErrorMessage = "El mes hasta debe estar entre 1 y 12.")]
        public int? MesHasta { get; set; }

        [Required(ErrorMessage = "El año hasta es obligatorio.")]
        [Range(2000, 2100, ErrorMessage = "El año hasta no es válido.")]
        public int? AnioHasta { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateTime? FechaVencimiento { get; set; }
    }
}
