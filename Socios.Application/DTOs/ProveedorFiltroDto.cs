namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de entrada con los filtros de búsqueda de proveedores.
    /// Se enlaza desde el body del endpoint GET /api/v1/proveedores.
    /// </summary>
    public class ProveedorFiltroDto
    {
        /// <summary>Texto que busca (contiene) por Nombre, Apellido o Razón Social.</summary>
        public string? Busqueda { get; set; }

        /// <summary>Texto que busca (contiene) por CUIT o CUIL.</summary>
        public string? CuitCuil { get; set; }

        /// <summary>
        /// Si es false (default) devuelve solo proveedores ACTIVOS.
        /// Si es true incluye también los INACTIVOS (ver todo).
        /// </summary>
        public bool IncluirInactivos { get; set; } = false;
    }
}
