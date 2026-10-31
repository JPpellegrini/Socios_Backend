using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos para asignar un nicho existente a un socio: el nicho, el socio y la financiación
    /// (heredada de <see cref="NichoFinanciacionDto"/>, la misma que usa la simulación de
    /// cuotas). Al asignarlo se cargan el valor, la financiación y la lápida, el nicho pasa a
    /// estar ocupado y se generan sus cuotas.
    /// </summary>
    public class NichoAsignarDto : NichoFinanciacionDto
    {
        [Required(ErrorMessage = "El id del nicho es obligatorio.")]
        public int? IdNicho { get; set; }

        [Required(ErrorMessage = "El id de la entidad del socio es obligatorio.")]
        public int? IdEntidad { get; set; }
    }
}
