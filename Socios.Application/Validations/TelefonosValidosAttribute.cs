using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Socios.Application.Validations
{
    /// <summary>
    /// Valida que la lista de teléfonos tenga al menos un elemento y que cada teléfono
    /// respete el formato esperado.
    /// </summary>
    public class TelefonosValidosAttribute : ValidationAttribute
    {
        public static readonly Regex Formato = new(@"^\+?[0-9\s\-()]{6,20}$", RegexOptions.Compiled);

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is not IEnumerable<string> telefonos || !telefonos.Any())
                return new ValidationResult("Debe cargar al menos un teléfono.");

            foreach (var telefono in telefonos)
            {
                if (string.IsNullOrWhiteSpace(telefono) || !Formato.IsMatch(telefono.Trim()))
                    return new ValidationResult($"El teléfono '{telefono}' no tiene un formato válido.");
            }

            return ValidationResult.Success;
        }
    }
}
