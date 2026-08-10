namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de entrada con los filtros de búsqueda de nichos.
    /// Se enlaza desde el query string del endpoint GET /api/v1/nichos.
    /// </summary>
    public class NichoFiltroDto
    {
        public string? Busqueda { get; set; }
    }
}
