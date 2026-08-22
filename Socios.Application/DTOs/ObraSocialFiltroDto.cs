namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de entrada con los filtros de búsqueda de obras sociales.
    /// Se enlaza desde el query string del endpoint GET /api/v1/obras-sociales.
    /// </summary>
    public class ObraSocialFiltroDto
    {
        /// <summary>Texto que busca (contiene) por Nombre de obra social.</summary>
        public string? Busqueda { get; set; }
    }
}