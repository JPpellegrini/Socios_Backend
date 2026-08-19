using System.ComponentModel.DataAnnotations;
using Socios.Application.Validations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos que se pueden modificar de un proveedor ya existente.
    ///
    /// A diferencia del alta, acá NO se toca la identidad (tipo de documento y documento):
    /// solo razón social, fecha de nacimiento, servicio prestado, domicilio y contactos.
    /// Las validaciones de formato replican las del alta para que un dato válido al crearlo
    /// lo siga siendo.
    /// </summary>
    public class ProveedorModificarDto
    {
        [Required(ErrorMessage = "El id de la entidad es obligatorio.")]
        public int IdEntidad { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [RegularExpression(@"^.{3,250}$", ErrorMessage = "La razón social debe tener entre 3 y 250 caracteres.")]
        public string RazonSocial { get; set; } = null!;

        /// <summary>
        /// Fecha de nacimiento / constitución del proveedor (cuándo se fundó la empresa o
        /// nació el monotributista). Opcional: puede no informarse.
        /// </summary>
        public DateTime? FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El servicio prestado es obligatorio.")]
        public int? IdPrestacion { get; set; }

        // Domicilio (vive en la entidad)
        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        public int? IdCiudad { get; set; }

        [Required(ErrorMessage = "La calle es obligatoria.")]
        public string Calle { get; set; } = null!;

        [Required(ErrorMessage = "La altura es obligatoria.")]
        public int? Altura { get; set; }

        public string? Observaciones { get; set; }

        // Contactos (reemplazan por completo a los actuales)
        [TelefonosValidos]
        public List<string> Telefonos { get; set; } = new();

        [EmailsValidos]
        public List<string>? Emails { get; set; }
    }
}
