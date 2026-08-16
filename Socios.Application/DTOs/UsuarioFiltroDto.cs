namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de entrada con los filtros de búsqueda de socios.
    /// Se enlaza desde el query string del endpoint GET /api/v1/socios.
    /// </summary>
    public class UsuarioFiltroDto
    {
        public string? Usuario { get; set; }
        public string? Rol { get; set; }
    }
}
