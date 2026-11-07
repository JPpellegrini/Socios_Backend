using System.ComponentModel.DataAnnotations;

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

        /// <summary>Máximo que admite la columna Saldo (numeric(18,2)).</summary>
        private const decimal SaldoMaximo = 9_999_999_999_999_999.99m;

        // Los límites se validan acá y no con [Range(typeof(decimal), ...)]: ese atributo
        // interpreta los textos con la cultura del servidor (es-AR toma el punto como miles) y
        // revienta con 500.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!SaldoInicial.HasValue)
                yield break;

            if (SaldoInicial.Value < 0)
                yield return new ValidationResult(
                    "El saldo inicial no puede ser negativo.", new[] { nameof(SaldoInicial) });
            else if (SaldoInicial.Value > SaldoMaximo)
                yield return new ValidationResult(
                    "El saldo inicial excede el máximo permitido.", new[] { nameof(SaldoInicial) });

            // La columna guarda 2 decimales: no se redondea en silencio lo que declaró el usuario.
            if (decimal.Round(SaldoInicial.Value, 2) != SaldoInicial.Value)
                yield return new ValidationResult(
                    "El saldo inicial admite como máximo 2 decimales.",
                    new[] { nameof(SaldoInicial) });
        }
    }
}
