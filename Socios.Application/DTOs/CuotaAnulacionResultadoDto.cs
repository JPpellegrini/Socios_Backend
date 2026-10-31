namespace Socios.Application.DTOs
{
    /// <summary>
    /// Resultado de anular una cuota. Para una cuota de socio/sepelio se anula solo esa; para
    /// una de nicho se anula todo el lote y se libera el nicho asignado.
    /// </summary>
    public class CuotaAnulacionResultadoDto
    {
        /// <summary>Mensaje listo para mostrar al usuario.</summary>
        public string Mensaje { get; set; } = null!;

        /// <summary>Cantidad de cuotas que pasaron a ANULADA.</summary>
        public int CuotasAnuladas { get; set; }

        /// <summary>Id del nicho que quedó libre, si la anulación fue de cuotas de nicho.</summary>
        public int? IdNichoLiberado { get; set; }
    }
}
