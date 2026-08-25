namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de entrada con los filtros de búsqueda de nichos.
    /// Se enlaza desde el query string del endpoint GET /api/v1/nichos.
    /// </summary>
    public class NichoFiltroDto
    {
        /// <summary>
        /// Campo de búsqueda del socio. Matchea contra nombre, apellido o DNI.
        /// Si viene vacío, no filtra.
        /// </summary>
        public string? Busqueda { get; set; }

        /// <summary>
        /// Campo de búsqueda del nicho. Matchea contra sector o número.
        /// Si viene vacío, no filtra.
        /// </summary>
        public string? SectorNumero { get; set; }

        /// <summary>
        /// Filtro de ocupación para el checkbox del front.
        /// false = solo libres, true = todos (incluye ocupados).
        /// </summary>
        public bool Ocupado { get; set; }
    }
}
