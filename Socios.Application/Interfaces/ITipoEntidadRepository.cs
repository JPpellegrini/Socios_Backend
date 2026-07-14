namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Acceso a los tipos de entidad (catálogo: "Socio", "Proveedor", etc.).
    /// </summary>
    public interface ITipoEntidadRepository
    {
        /// <summary>
        /// Devuelve el Id del tipo de entidad con ese nombre (por ejemplo "Socio"),
        /// o null si no existe. Evita dejar números mágicos en el código.
        /// </summary>
        Task<int?> ObtenerIdPorNombreAsync(string nombre);
    }
}
