using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Filtros de la búsqueda de cuotas.
    /// Se enlaza desde el query string del endpoint GET /api/v1/cuotas.
    /// Todos los filtros son opcionales y se combinan con AND.
    /// </summary>
    public class CuotaFiltroDto
    {
        /// <summary>
        /// Campo único de socio: matchea (contiene, case-insensitive) por Nombre,
        /// Apellido o Documento (DNI). Vacío no filtra.
        /// </summary>
        public string? Busqueda { get; set; }

        /// <summary>
        /// Tipo de cuota a filtrar por concepto. Valores válidos: SOCIO, SEPELIO, NICHO.
        /// "TODOS" (o no enviar el parámetro) no filtra.
        /// El null en la lista es obligatorio: [AllowedValues] rechaza null si no está.
        /// </summary>
        [AllowedValues(null, "TODOS", "SOCIO", "SEPELIO", "NICHO",
            ErrorMessage = "El tipo de cuota debe ser TODOS, SOCIO, SEPELIO o NICHO.")]
        public string? TipoCuota { get; set; }

        /// <summary>
        /// Estado a filtrar. Valores válidos: los que guarda la columna Estado
        /// (PAGADO, PENDIENTE, VENCIDA, ANULADA). "TODAS" (o no enviar el parámetro) no filtra.
        /// El null en la lista es obligatorio: [AllowedValues] rechaza null si no está.
        /// </summary>
        [AllowedValues(null, "TODAS", "PAGADO", "PENDIENTE", "VENCIDA", "ANULADA",
            ErrorMessage = "El estado debe ser TODAS, PAGADO, PENDIENTE, VENCIDA o ANULADA.")]
        public string? Estado { get; set; }
    }
}
