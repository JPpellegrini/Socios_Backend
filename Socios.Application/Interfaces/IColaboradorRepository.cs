using Socios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface IColaboradorRepository
    {
        Task<IEnumerable<Colaborador>> GetAllAsync();
        Task<Colaborador?> GetByIdAsync(int idColaborador);
        Task<Colaborador> AddAsync(Colaborador colaborador);
        Task UpdateAsync(Colaborador colaborador);
        Task DeleteAsync(int idColaborador);
        Task<List<Entidad>> BuscarAsync(string? busqueda);
        Task<EntidadTipo?> ObtenerEntidadTipoAsync(int idEntidadTipo);
        void ActualizarEntidadTipo(EntidadTipo entidadTipo);
        Task RegistrarBajaAsync(EntidadBaja baja);

    }
}
