using Socios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface IEntidadTipoRepository
    {
        Task<IEnumerable<EntidadTipo>> GetAllAsync();
        Task<EntidadTipo?> GetByIdAsync(int idEntidad, int idTipo);
        Task<EntidadTipo> AddAsync(EntidadTipo entidadTipo);
        Task UpdateAsync(EntidadTipo entidadTipo);
        Task DeleteAsync(int idEntidad, int idTipo);
    }
}
