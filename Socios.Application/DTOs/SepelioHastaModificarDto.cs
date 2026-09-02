using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para modificar la cuota de SEPELIO "hasta el tope" (la que tiene límite de edad):
    /// se cambia el importe y el tope de edad (ej. hasta 65 años). El id tiene que corresponder
    /// a un tipo de cuota SEPELIO con tope (se valida como regla de negocio en el caso de uso).
    /// La fecha de última modificación se actualiza automáticamente en el backend.
    ///
    /// El tope que se guarda acá es la única frontera del plan: la cuota "más de" hereda ese
    /// número, así que al editarlo también cambia el "MAS DE n" que se muestra en la grilla.
    /// </summary>
    public class SepelioHastaModificarDto
    {
        [Required(ErrorMessage = "El id del tipo de cuota es obligatorio.")]
        public int? Id_TipoCuota { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El importe no puede ser negativo.")]
        public decimal? Importe { get; set; }

        [Required(ErrorMessage = "El tope de edad es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El tope de edad debe ser mayor a cero.")]
        public int? EdadTope { get; set; }
    }
}
