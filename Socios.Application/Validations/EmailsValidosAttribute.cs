using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Socios.Application.Validations
{
    /// <summary>
    /// Valida el formato de cada email de la lista. La lista es opcional: null o vacía es válido.
    /// </summary>
    public class EmailsValidosAttribute : ValidationAttribute
    {
        public static readonly Regex Formato = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is not IEnumerable<string> emails)
                return ValidationResult.Success;

            foreach (var email in emails)
            {
                if (string.IsNullOrWhiteSpace(email) || !Formato.IsMatch(email.Trim()))
                    return new ValidationResult($"El email '{email}' no tiene un formato válido.");
            }

            return ValidationResult.Success;
        }
    }
}
