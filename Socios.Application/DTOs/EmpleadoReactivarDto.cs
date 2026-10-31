using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para reactivar un empleado: solo el Id de la entidad, que debe existir
    /// y estar INACTIVA como empleado. La reactivación únicamente pasa el estado a ACTIVO.
    /// </summary>
    public class EmpleadoReactivarDto
    {
        [Required(ErrorMessage = "El id de la entidad es obligatorio.")]
        public int IdEntidad { get; set; }
    }
}
