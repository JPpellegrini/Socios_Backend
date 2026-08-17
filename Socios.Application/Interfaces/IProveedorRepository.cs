using Socios.Application.DTOs;
using Socios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface IProveedorRepository
    {
        /// <summary>Lista los proveedores aplicando los filtros recibidos.</summary>
        Task<List<ProveedorListadoDto>> BuscarAsync(ProveedorFiltroDto filtro);

        /// <summary>
        /// Trae todos los datos de un proveedor (por Id de entidad) para visualizarlo,
        /// o null si esa entidad no es proveedor.
        /// </summary>
        Task<ProveedorDetalleDto?> ObtenerDetalleAsync(int idEntidad);

        /// <summary>Indica si la entidad indicada ya está registrada como proveedor.</summary>
        Task<bool> EsProveedorAsync(int idEntidad);

        /// <summary>
        /// Trae el proveedor (con su entidad asociada) de una entidad, trackeado por EF
        /// para poder modificarlo. Devuelve null si esa entidad no es proveedor.
        /// </summary>
        Task<Proveedor?> ObtenerConEntidadPorEntidadAsync(int idEntidad);

        /// <summary>Marca el proveedor para ser insertado. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Proveedor proveedor);
    }
}
