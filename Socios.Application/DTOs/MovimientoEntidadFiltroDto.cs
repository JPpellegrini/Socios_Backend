using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Filtro del reporte de movimientos por entidad (proveedores y colaboradores).
    /// La entidad viaja en la ruta; acá van el tipo y el período.
    /// </summary>
    public class MovimientoEntidadFiltroDto
    {
        /// <summary>"PROVEEDOR" o "COLABORADOR".</summary>
        [Required(ErrorMessage = "El tipo de entidad es obligatorio.")]
        public string? Tipo { get; set; }

        /// <summary>Inicio del período (inclusive).</summary>
        [Required(ErrorMessage = "La fecha desde es obligatoria.")]
        public DateTime? FechaDesde { get; set; }

        /// <summary>Fin del período (inclusive: se toma el día completo).</summary>
        [Required(ErrorMessage = "La fecha hasta es obligatoria.")]
        public DateTime? FechaHasta { get; set; }
    }
}
