using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para dar de alta un nicho. Solo se informan sector y número: el nicho nace
    /// libre (Ocupado = "NO") y sin lápida (ConLapida = "NO"). El valor y los datos de
    /// financiación se completan recién al asignar el nicho a un socio.
    /// </summary>
    public class NichoCrearDto
    {
        [Required(ErrorMessage = "El sector es obligatorio.")]
        [StringLength(100, ErrorMessage = "El sector no puede superar los 100 caracteres.")]
        public string Sector { get; set; } = null!;

        [Required(ErrorMessage = "El número de nicho es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número de nicho no puede superar los 50 caracteres.")]
        public string NroNicho { get; set; } = null!;
    }
}
