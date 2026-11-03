using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para cobrar una cuota. El monto NO se recibe: lo calcula el backend según el
    /// estado de la cuota (pendiente: su monto; vencida: el valor actual de configuración).
    /// La cuenta solo se informa si el método de pago no es efectivo (con efectivo, la
    /// cuenta es siempre EFECTIVO).
    /// </summary>
    public class CuotaPagarDto
    {
        [Required(ErrorMessage = "El id de la cuota es obligatorio.")]
        public int? IdCuota { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        public int? IdMetodoPago { get; set; }

        /// <summary>Cuenta donde ingresa el dinero. Obligatoria si el método no es efectivo.</summary>
        public int? IdCuenta { get; set; }
    }
}
