using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de entrada para filtrar el informe de altas y bajas de socios.
    /// </summary>
    public class SocioMovimientoFiltroDto
    {
        /// <summary>
        /// Inicio del período (inclusive).
        /// </summary>
        [Required(ErrorMessage = "La fecha desde es obligatoria.")]
        public DateTime? FechaDesde { get; set; }

        /// <summary>
        /// Fin del período (inclusive).
        /// </summary>
        [Required(ErrorMessage = "La fecha hasta es obligatoria.")]
        public DateTime? FechaHasta { get; set; }

        /// <summary>
        /// Movimientos a incluir: "TODOS", "ALTAS" o "BAJAS".
        /// "ALTAS" trae solo altas vigentes: excluye a los socios que ya fueron dados de baja.
        /// </summary>
        public string Tipo { get; set; } = "TODOS";
    }
}
