using System.ComponentModel.DataAnnotations;

namespace Socios.Application.Validations
{
    /// <summary>
    /// Validación común del efectivo que declara el cajero (saldo de apertura y de cierre):
    /// no negativo, dentro de lo que admite la columna Saldo (numeric(18,2)) y con 2 decimales
    /// como máximo (no se redondea en silencio lo que declaró).
    ///
    /// Se valida acá y no con [Range(typeof(decimal), ...)]: ese atributo interpreta los textos
    /// con la cultura del servidor (es-AR toma el punto como separador de miles) y revienta con 500.
    /// </summary>
    public static class ValidacionSaldoEfectivo
    {
        private const decimal SaldoMaximo = 9_999_999_999_999_999.99m;

        public static IEnumerable<ValidationResult> Validar(decimal? saldo, string nombreCampo, string etiqueta)
        {
            if (!saldo.HasValue)
                yield break;

            if (saldo.Value < 0)
                yield return new ValidationResult($"El {etiqueta} no puede ser negativo.", new[] { nombreCampo });
            else if (saldo.Value > SaldoMaximo)
                yield return new ValidationResult($"El {etiqueta} excede el máximo permitido.", new[] { nombreCampo });

            if (decimal.Round(saldo.Value, 2) != saldo.Value)
                yield return new ValidationResult($"El {etiqueta} admite como máximo 2 decimales.", new[] { nombreCampo });
        }
    }
}
