using Socios.Application.DTOs;
using Socios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface IEntidadRepository
    {
        Task<IEnumerable<Entidad>> GetAllAsync();
        Task<Entidad?> GetByIdAsync(int id);
        Task<Entidad> AddAsync(Entidad entidad);
        Task UpdateAsync(Entidad entidad);
        Task DeleteAsync(int id);
        Task<EntidadDto> BuscarAsync(EntidadFiltroDto filtro);
    }
}
