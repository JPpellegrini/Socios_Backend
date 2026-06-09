using Socios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface ISocioRepository
    {
        Task<IEnumerable<Socio>> GetAllAsync();
        Task<Socio?> GetByIdAsync(int idEntidad);
        Task<Socio> AddAsync(Socio socio);
        Task UpdateAsync(Socio socio);
        Task DeleteAsync(int idEntidad);
    }
}
