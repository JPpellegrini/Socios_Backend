using System.ComponentModel.DataAnnotations;
using Socios.Application.Validations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para dar de alta un proveedor. Comparte validaciones con el alta de socio
    /// en los campos comunes (domicilio, contactos). La identidad admite DNI o CUIT: el
    /// largo exacto según el tipo se valida como regla de negocio en el caso de uso.
    /// </summary>
    public class ProveedorCrearDto
    {
        [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
        [AllowedValues("DNI", "CUIT", ErrorMessage = "El tipo de documento debe ser 'DNI' o 'CUIT'.")]
        public string TipoDocumento { get; set; } = null!;

        [Required(ErrorMessage = "El documento es obligatorio.")]
        [RegularExpression(@"^\d{7,11}$", ErrorMessage = "El documento debe tener entre 7 y 11 dígitos.")]
        public string Documento { get; set; } = null!;

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [RegularExpression(@"^.{3,250}$", ErrorMessage = "La razón social debe tener entre 3 y 250 caracteres.")]
        public string RazonSocial { get; set; } = null!;

        [Required(ErrorMessage = "El servicio prestado es obligatorio.")]
        public int? IdPrestacion { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        public int? IdCiudad { get; set; }

        [Required(ErrorMessage = "La calle es obligatoria.")]
        public string Calle { get; set; } = null!;

        [Required(ErrorMessage = "La altura es obligatoria.")]
        public int? Altura { get; set; }

        public string? Observaciones { get; set; }

        [TelefonosValidos]
        public List<string> Telefonos { get; set; } = new();

        [EmailsValidos]
        public List<string>? Emails { get; set; }
    }
}
