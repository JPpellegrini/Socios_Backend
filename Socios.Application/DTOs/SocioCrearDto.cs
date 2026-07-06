using System.ComponentModel.DataAnnotations;
using Socios.Application.Validations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de entrada para el alta de un socio. El alta crea en cascada la Entidad,
    /// el Socio, su EntidadTipo (tipo "Socio", Estado ACTIVO) y los Contactos.
    /// Las validaciones se ejecutan automáticamente por [ApiController] antes de llegar
    /// al controlador (devuelve 400 con el detalle si algo falla).
    /// </summary>
    public class SocioCrearDto
    {
        // --- Documento / identidad ---
        public string? TipoDocumento { get; set; } // opcional; si viene vacío se asume "DNI"

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 u 8 dígitos.")]
        public string Dni { get; set; } = null!;

        // --- Datos personales ---
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime? FechaNacimiento { get; set; }

        // --- Domicilio ---
        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        public int? IdCiudad { get; set; }

        [Required(ErrorMessage = "La calle es obligatoria.")]
        public string Calle { get; set; } = null!;

        [Required(ErrorMessage = "La altura es obligatoria.")]
        public int? Altura { get; set; }

        public string? Observaciones { get; set; }

        // --- Datos de socio (opcionales) ---
        public int? IdObraSocial { get; set; }

        public string? NumeroAfiliado { get; set; }

        [AllowedValues(null, "A", "B", ErrorMessage = "El plan debe ser 'A' o 'B'.")]
        public string? Plan { get; set; }

        [AllowedValues(null, "SI", "NO", ErrorMessage = "El sepelio debe ser 'SI' o 'NO'.")]
        public string? Sepelio { get; set; }

        [AllowedValues(null, "SI", "NO", ErrorMessage = "El cobrador debe ser 'SI' o 'NO'.")]
        public string? Cobrador { get; set; }

        // --- Contactos ---
        [TelefonosValidos]
        public List<string> Telefonos { get; set; } = new();

        [EmailsValidos]
        public List<string>? Emails { get; set; }
    }
}
