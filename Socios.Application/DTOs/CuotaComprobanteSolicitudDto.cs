using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Pedido de comprobantes de cuotas adeudadas: los Id de las cuotas (Id_Deuda) a imprimir.
    /// Se emite un comprobante por cuota.
    /// </summary>
    public class CuotaComprobanteSolicitudDto
    {
        [Required(ErrorMessage = "Debe indicar al menos una cuota.")]
        [MinLength(1, ErrorMessage = "Debe indicar al menos una cuota.")]
        public List<int>? IdsCuota { get; set; }
    }
}
