using System.ComponentModel.DataAnnotations;
using Socios.Application.Validations;

namespace Socios.Application.DTOs
{
    public class SocioCrearDto
    {
        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 u 8 dígitos.")]
        public string Dni { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^.{3,150}$", ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [RegularExpression(@"^.{3,150}$", ErrorMessage = "El apellido debe tener entre 3 y 150 caracteres.")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime? FechaNacimiento { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        public int? IdCiudad { get; set; }

        [Required(ErrorMessage = "La calle es obligatoria.")]
        public string Calle { get; set; } = null!;

        [Required(ErrorMessage = "La altura es obligatoria.")]
        public int? Altura { get; set; }

        public string? Observaciones { get; set; }

        public int? IdObraSocial { get; set; }

        public string? NumeroAfiliado { get; set; }

        [AllowedValues("A", "B", ErrorMessage = "El plan debe ser 'A' o 'B'.")]
        public string? Plan { get; set; }

        [AllowedValues("SI", "NO", ErrorMessage = "El sepelio debe ser 'SI' o 'NO'.")]
        public string? Sepelio { get; set; }

        [AllowedValues("SI", "NO", ErrorMessage = "El cobrador debe ser 'SI' o 'NO'.")]
        public string? Cobrador { get; set; }

        [TelefonosValidos]
        public List<string> Telefonos { get; set; } = new();

        [EmailsValidos]
        public List<string>? Emails { get; set; }

        /// <summary>
        /// Codeudores que avalan al socio. Un socio no puede darse de alta sin al menos uno.
        /// Cada valor es el Id_Entidad de una persona (codeudor) que ya existe.
        /// </summary>
        [Required(ErrorMessage = "Debe asignar al menos un codeudor.")]
        [MinLength(1, ErrorMessage = "Debe asignar al menos un codeudor.")]
        public List<int> Codeudores { get; set; } = new();
    }
}
