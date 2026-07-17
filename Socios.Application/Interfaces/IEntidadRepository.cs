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
        Task<EntidadDto?> BuscarAsync(EntidadFiltroDto filtro);

        /// <summary>Trae la entidad que tenga exactamente ese DNI, o null si no existe.</summary>
        Task<Entidad?> ObtenerPorDniAsync(string dni);

        /// <summary>Marca la entidad para ser insertada. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Entidad entidad);
    }
}
