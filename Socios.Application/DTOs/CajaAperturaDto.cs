using System.ComponentModel.DataAnnotations;
using Socios.Application.Validations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para abrir la caja diaria: solo el efectivo con el que se inicia. El usuario y la
    /// fecha/hora NO se reciben (decisión del usuario 2026-11-07): la caja se abre a nombre del
    /// usuario autenticado y con la hora del servidor. El número de caja lo asigna el backend
    /// (es correlativo).
    /// </summary>
    public class CajaAperturaDto : IValidatableObject
    {
        /// <summary>Efectivo con el que el usuario inicia la caja (puede ser 0).</summary>
        [Required(ErrorMessage = "El saldo inicial es obligatorio.")]
        public decimal? SaldoInicial { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
            ValidacionSaldoEfectivo.Validar(SaldoInicial, nameof(SaldoInicial), "saldo inicial");
    }
}
