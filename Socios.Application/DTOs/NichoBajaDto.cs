using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para dar de baja un nicho: solo el Id del nicho, que debe existir y no
    /// estar ocupado. La baja elimina físicamente el nicho.
    /// </summary>
    public class NichoBajaDto
    {
        [Required(ErrorMessage = "El id del nicho es obligatorio.")]
        public int IdNicho { get; set; }
    }
}
