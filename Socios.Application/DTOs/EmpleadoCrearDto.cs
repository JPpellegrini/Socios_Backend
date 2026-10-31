using System.ComponentModel.DataAnnotations;
using Socios.Application.Validations;

namespace Socios.Application.DTOs
{
    public class EmpleadoCrearDto
    {
        public int Id_Entidad { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 u 8 dígitos.")]
        public string Dni { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^.{3,150}$", ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [RegularExpression(@"^.{3,150}$", ErrorMessage = "El apellido debe tener entre 3 y 150 caracteres.")]
        public string Apellido { get; set; } = null!;
        public string? Sexo { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime? Nacimiento { get; set; }

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
