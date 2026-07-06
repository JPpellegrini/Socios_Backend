using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    public class EntidadFiltroDto
    {
        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 u 8 dígitos.")]
        public string? Dni { get; set; }
    }
}
