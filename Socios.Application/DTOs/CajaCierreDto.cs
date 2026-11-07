using System.ComponentModel.DataAnnotations;
using Socios.Application.Validations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para cerrar la caja diaria: solo el efectivo que el cajero cuenta al final. El
    /// usuario y la fecha/hora no se reciben (salen del login y del servidor), igual que en la
    /// apertura.
    /// </summary>
    public class CajaCierreDto : IValidatableObject
    {
        /// <summary>Efectivo contado al cerrar.</summary>
        [Required(ErrorMessage = "El saldo final es obligatorio.")]
        public decimal? SaldoFinal { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
            ValidacionSaldoEfectivo.Validar(SaldoFinal, nameof(SaldoFinal), "saldo final");
    }
}
