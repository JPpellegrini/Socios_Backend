using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para generar las cuotas de UN mes/año para todos los socios activos.
    /// El vencimiento se recibe del cliente y se aplica a todas las cuotas generadas;
    /// el caso de uso valida que sea posterior a hoy.
    /// </summary>
    public class CuotaGenerarMasivaDto
    {
        [Required(ErrorMessage = "El mes es obligatorio.")]
        [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12.")]
        public int? Mes { get; set; }

        [Required(ErrorMessage = "El año es obligatorio.")]
        [Range(2000, 2100, ErrorMessage = "El año no es válido.")]
        public int? Anio { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateTime? FechaVencimiento { get; set; }
    }
}
