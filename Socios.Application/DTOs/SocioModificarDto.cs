using System.ComponentModel.DataAnnotations;
using Socios.Application.Validations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos que se pueden modificar de un socio ya existente.
    ///
    /// A diferencia del alta, acá NO se tocan los datos de identidad (DNI, nombre,
    /// apellido, fecha de nacimiento): solo domicilio, datos de socio y contactos.
    /// Las validaciones de formato replican las del alta para que un dato que era
    /// válido al crearlo lo siga siendo al editarlo.
    /// </summary>
    public class SocioModificarDto
    {
        [Required(ErrorMessage = "El id del socio es obligatorio.")]
        public int IdSocio { get; set; }

        // Domicilio (vive en la entidad)
        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        public int? IdCiudad { get; set; }

        [Required(ErrorMessage = "La calle es obligatoria.")]
        public string Calle { get; set; } = null!;

        [Required(ErrorMessage = "La altura es obligatoria.")]
        public int? Altura { get; set; }

        public string? Observaciones { get; set; }

        // Datos de socio
        public int? IdObraSocial { get; set; }

        public string? NumeroAfiliado { get; set; }

        [AllowedValues("A", "B", ErrorMessage = "El plan debe ser 'A' o 'B'.")]
        public string? Plan { get; set; }

        [AllowedValues("SI", "NO", ErrorMessage = "El sepelio debe ser 'SI' o 'NO'.")]
        public string? Sepelio { get; set; }

        [AllowedValues("SI", "NO", ErrorMessage = "El cobrador debe ser 'SI' o 'NO'.")]
        public string? Cobrador { get; set; }

        // Contactos (reemplazan por completo a los actuales)
        [TelefonosValidos]
        public List<string> Telefonos { get; set; } = new();

        [EmailsValidos]
        public List<string>? Emails { get; set; }
    }
}
