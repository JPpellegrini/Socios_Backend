using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para modificar la cuota de SEPELIO "más de" (la que NO tiene tope de edad propio:
    /// va desde el tope de su hermana hasta el infinito). Solo se cambia el importe. El id tiene
    /// que corresponder a un tipo de cuota SEPELIO sin tope (se valida como regla de negocio en
    /// el caso de uso). La fecha de última modificación se actualiza automáticamente en el backend.
    /// </summary>
    public class SepelioMasDeModificarDto
    {
        [Required(ErrorMessage = "El id del tipo de cuota es obligatorio.")]
        public int? Id_TipoCuota { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El importe no puede ser negativo.")]
        public decimal? Importe { get; set; }
    }
}
