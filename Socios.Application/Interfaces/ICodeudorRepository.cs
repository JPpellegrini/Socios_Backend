using Socios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface ICodeudorRepository
    {
        Task<IEnumerable<Codeudor>> GetAllAsync();
        Task<Codeudor?> GetByIdAsync(int idEntidad, int idEntidadCodeudor);
        Task<Codeudor> AddAsync(Codeudor codeudor);
        Task UpdateAsync(Codeudor codeudor);
        Task DeleteAsync(int idEntidad, int idEntidadCodeudor);
    }
}
