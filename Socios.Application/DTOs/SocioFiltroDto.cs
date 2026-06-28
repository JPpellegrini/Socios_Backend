namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de entrada con los filtros de búsqueda de socios.
    /// Se enlaza desde el query string del endpoint GET /api/v1/socios.
    /// </summary>
    public class SocioFiltroDto
    {
        /// <summary>Texto que busca (contiene) por Nombre o Apellido.</summary>
        public string? Busqueda { get; set; }

        /// <summary>Texto que busca (contiene) por DNI.</summary>
        public string? Dni { get; set; }

        /// <summary>
        /// Si es false (default) devuelve solo socios ACTIVOS.
        /// Si es true incluye también los INACTIVOS.
        /// </summary>
        public bool IncluirInactivos { get; set; } = false;
    }
}
