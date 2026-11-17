using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>Prestación (servicio que brinda un proveedor o colaborador), para listados y desplegables.</summary>
    public class PrestacionDto
    {
        public int IdPrestacion { get; set; }
        public string Nombre { get; set; } = null!;

        /// <summary>COLABORACION o PRESTACION.</summary>
        public string Tipo { get; set; } = null!;

        /// <summary>ACTIVA o INACTIVA. Una prestación INACTIVA no se puede asignar a proveedores ni colaboradores nuevos.</summary>
        public string Estado { get; set; } = null!;
    }

    /// <summary>Filtros del listado de prestaciones.</summary>
    public class PrestacionFiltroDto
    {
        /// <summary>COLABORACION o PRESTACION. Vacío = ambos tipos.</summary>
        [AllowedValues(null, "COLABORACION", "PRESTACION", ErrorMessage = "El tipo debe ser 'COLABORACION' o 'PRESTACION'.")]
        public string? Tipo { get; set; }

        /// <summary>
        /// ACTIVA (por defecto: las que se pueden asignar, para los desplegables), INACTIVA o TODOS.
        /// </summary>
        [AllowedValues("ACTIVA", "INACTIVA", "TODOS", ErrorMessage = "El estado debe ser 'ACTIVA', 'INACTIVA' o 'TODOS'.")]
        public string Estado { get; set; } = "ACTIVA";

        /// <summary>
        /// Parte del nombre a buscar, sin distinguir mayúsculas ni tildes ("cardio" encuentra
        /// "CARDIOLOGÍA"). Vacío = sin filtrar por nombre.
        /// </summary>
        [StringLength(100, ErrorMessage = "El nombre a buscar no puede superar los 100 caracteres.")]
        public string? Nombre { get; set; }
    }

    /// <summary>Datos para dar de alta una prestación. Se crea siempre en estado ACTIVA.</summary>
    public class PrestacionCrearDto
    {
        /// <summary>Nombre de la prestación. Se guarda en mayúsculas y sin espacios de más.</summary>
        [Required(ErrorMessage = "El nombre de la prestación es obligatorio.")]
        [RegularExpression(@"^\s*\S.{1,98}\S\s*$", ErrorMessage = "El nombre de la prestación debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        /// <summary>COLABORACION o PRESTACION.</summary>
        [Required(ErrorMessage = "El tipo de la prestación es obligatorio.")]
        [AllowedValues("COLABORACION", "PRESTACION", ErrorMessage = "El tipo debe ser 'COLABORACION' o 'PRESTACION'.")]
        public string Tipo { get; set; } = null!;
    }

    /// <summary>
    /// Datos para modificar una prestación: nombre y tipo. El estado se cambia con la baja y la
    /// reactivación, no desde acá.
    /// </summary>
    public class PrestacionModificarDto
    {
        /// <summary>Nuevo nombre. Mismas reglas que en el alta.</summary>
        [Required(ErrorMessage = "El nombre de la prestación es obligatorio.")]
        [RegularExpression(@"^\s*\S.{1,98}\S\s*$", ErrorMessage = "El nombre de la prestación debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        /// <summary>COLABORACION o PRESTACION.</summary>
        [Required(ErrorMessage = "El tipo de la prestación es obligatorio.")]
        [AllowedValues("COLABORACION", "PRESTACION", ErrorMessage = "El tipo debe ser 'COLABORACION' o 'PRESTACION'.")]
        public string Tipo { get; set; } = null!;
    }
}
