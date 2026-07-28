namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de entrada con los filtros de búsqueda de ciudades.
    /// Se enlaza desde el query string del endpoint GET /api/v1/ciudades.
    /// </summary>
    public class CiudadFiltroDto
    {
        /// <summary>Texto que busca (contiene) por Nombre de ciudad.</summary>
        public string? Busqueda { get; set; }

        public class ErrorResponseDto
        {
            public string Mensaje { get; set; } = string.Empty;
        }
    }
}