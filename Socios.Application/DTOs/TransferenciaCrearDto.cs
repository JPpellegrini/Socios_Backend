using System.ComponentModel.DataAnnotations;
using Socios.Application.Validations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos de la pantalla "Transferencias entre Cuentas": mover dinero de una cuenta del
    /// Centro a otra (ej. depositar el efectivo en el banco, o extraer del banco al efectivo).
    /// Usuario y fecha/hora no se reciben: salen del login y del servidor.
    /// </summary>
    public class TransferenciaCrearDto : IValidatableObject
    {
        [Required(ErrorMessage = "La cuenta de origen es obligatoria.")]
        public int? IdCuentaOrigen { get; set; }

        [Required(ErrorMessage = "La cuenta de destino es obligatoria.")]
        public int? IdCuentaDestino { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio.")]
        public decimal? Importe { get; set; }

        /// <summary>
        /// Opcional: número de comprobante u operación del banco. Facilita conciliar con el
        /// extracto bancario. Se guarda en la observación de los movimientos.
        /// </summary>
        [MaxLength(50, ErrorMessage = "El número de comprobante admite como máximo 50 caracteres.")]
        public string? NroComprobante { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IdCuentaOrigen.HasValue && IdCuentaOrigen == IdCuentaDestino)
                yield return new ValidationResult(
                    "La cuenta de origen y la de destino no pueden ser la misma.",
                    new[] { nameof(IdCuentaDestino) });

            if (Importe.HasValue && Importe.Value == 0)
                yield return new ValidationResult("El importe debe ser mayor a cero.", new[] { nameof(Importe) });

            foreach (var error in ValidacionSaldoEfectivo.Validar(Importe, nameof(Importe), "importe"))
                yield return error;
        }
    }
}
