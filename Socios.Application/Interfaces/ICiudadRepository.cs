using Socios.Application.DTOs;
using Socios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface ICiudadRepository
    {
        Task<IEnumerable<Ciudad>> GetAllAsync();
        Task<Ciudad?> GetByIdAsync(int id);
        Task<List<CiudadListadoDto>> BuscarAsync(CiudadFiltroDto filtro);
    }
}