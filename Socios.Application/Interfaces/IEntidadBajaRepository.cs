using Socios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface IEntidadBajaRepository
    {
        Task<IEnumerable<EntidadBaja>> GetAllAsync();
        Task<EntidadBaja?> GetByIdAsync(int idBaja);
        Task<EntidadBaja> AddAsync(EntidadBaja entidadBaja);
        Task UpdateAsync(EntidadBaja entidadBaja);
        Task DeleteAsync(int idBaja);
    }
}
