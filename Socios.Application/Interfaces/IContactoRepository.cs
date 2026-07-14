using Socios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface IContactoRepository
    {
        Task<IEnumerable<Contacto>> GetAllAsync();
        Task<Contacto?> GetByIdAsync(int idContacto);
        Task<Contacto> AddAsync(Contacto contacto);
        Task UpdateAsync(Contacto contacto);
        Task DeleteAsync(int idContacto);

        /// <summary>Marca el contacto para ser insertado. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Contacto contacto);
    }
}
