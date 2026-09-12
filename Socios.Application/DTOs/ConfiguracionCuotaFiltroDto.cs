namespace Socios.Application.DTOs
{
    /// <summary>
    /// Filtros de la búsqueda de configuración de cuotas.
    /// Se enlaza desde el query string del endpoint GET /api/v1/configuracion-cuotas.
    /// </summary>
    public class ConfiguracionCuotaFiltroDto
    {
        /// <summary>
        /// Concepto a filtrar: "socio" o "sepelio". Vacío o "todos" no filtra
        /// (incluye también el concepto NICHO).
        /// </summary>
        public string? Concepto { get; set; }
    }
}
