using Socios.Application.DTOs;

namespace Socios.Application.UseCases.Proveedores
{
    /// <summary>
    /// Punto de entrada de los casos de uso del proveedor.
    ///
    /// Define QUÉ operaciones existen, sin atarse a CÓMO se resuelven por dentro.
    /// El controlador depende de esta interfaz, no de la implementación concreta.
    /// </summary>
    public interface IProveedorUseCase
    {
        /// <summary>Da de alta un proveedor y devuelve el Id generado.</summary>
        Task<int> CrearAsync(ProveedorCrearDto dto);
    }
}
