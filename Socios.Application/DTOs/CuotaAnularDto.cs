using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para anular una cuota: solo el Id de la cuota (Id_Deuda). La anulación es
    /// lógica (pasa el estado a ANULADA), no borra la fila. Las reglas (dentro de la hora
    /// de generada y que no esté paga) se validan en el caso de uso.
    /// </summary>
    public class CuotaAnularDto
    {
        [Required(ErrorMessage = "El id de la cuota es obligatorio.")]
        public int? IdCuota { get; set; }
    }
}
