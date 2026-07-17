using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    public class SocioBajaDto
    {
        [Required(ErrorMessage = "El motivo de la baja es obligatorio.")]
        [StringLength(250, ErrorMessage = "El motivo no puede superar los 250 caracteres.")]
        public string Motivo { get; set; } = null!;
    }
}
