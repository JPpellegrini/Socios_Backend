using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>Prestación (servicio que brinda un proveedor o colaborador), para listados y desplegables.</summary>
    public class PrestacionDto
    {
        public int IdPrestacion { get; set; }
        public string Nombre { get; set; } = null!;
    }

    /// <summary>Datos para dar de alta una prestación.</summary>
    public class PrestacionCrearDto
    {
        /// <summary>Nombre de la prestación. Se guarda en mayúsculas y sin espacios de más.</summary>
        [Required(ErrorMessage = "El nombre de la prestación es obligatorio.")]
        [RegularExpression(@"^\s*\S.{1,98}\S\s*$", ErrorMessage = "El nombre de la prestación debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = null!;
    }
}
