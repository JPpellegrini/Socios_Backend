using Socios.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface IProveedorRepository
    {
        /// <summary>Lista los proveedores aplicando los filtros recibidos.</summary>
        Task<List<ProveedorListadoDto>> BuscarAsync(ProveedorFiltroDto filtro);
    }
}
