using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para dar de baja un proveedor: solo el Id de la entidad, que debe existir
    /// y estar ACTIVA como proveedor. La baja únicamente pasa el estado a INACTIVO.
    /// </summary>
    public class ProveedorBajaDto
    {
        [Required(ErrorMessage = "El id de la entidad es obligatorio.")]
        public int IdEntidad { get; set; }
    }
}
