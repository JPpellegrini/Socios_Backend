using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    public class SocioBajaDto
    {
        [Required(ErrorMessage = "El id del socio es obligatorio.")]
        public int IdSocio { get; set; } = 0;

        [Required(ErrorMessage = "El motivo de la baja es obligatorio.")]
        [StringLength(250, ErrorMessage = "El motivo no puede superar los 250 caracteres.")]
        [AllowedValues("MORA", "FALLECIMIENTO", "RENUNCIA", ErrorMessage = "El motivo debe ser 'MORA', 'FALLECIMIENTO' o 'RENUNCIA'.")]
        public string Motivo { get; set; } = null!;
    }
}
