using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para modificar la cuota del concepto SOCIO: solo se cambia el importe.
    /// El id tiene que corresponder a un tipo de cuota de concepto SOCIO (se valida como
    /// regla de negocio en el caso de uso). La fecha de última modificación se actualiza
    /// automáticamente en el backend, no se recibe del cliente.
    /// </summary>
    public class ConfiguracionCuotaModificarDto
    {
        [Required(ErrorMessage = "El id del tipo de cuota es obligatorio.")]
        public int? Id_TipoCuota { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El importe no puede ser negativo.")]
        public decimal? Importe { get; set; }
    }
}
