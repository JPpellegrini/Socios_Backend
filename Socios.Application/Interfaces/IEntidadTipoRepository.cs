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

        /// <summary>Marca el registro para ser insertado. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(EntidadTipo entidadTipo);
    }
}
