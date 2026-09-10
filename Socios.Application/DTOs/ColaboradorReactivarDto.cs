using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para reactivar un proveedor: solo el Id de la entidad, que debe existir
    /// y estar INACTIVA como proveedor. La reactivación únicamente pasa el estado a ACTIVO.
    /// </summary>
    public class ColaboradorReactivarDto
    {
        [Required(ErrorMessage = "El id de la entidad es obligatorio.")]
        public int IdEntidad { get; set; }
    }
}
