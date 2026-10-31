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
        /// <summary>Indica si la entidad indicada ya está registrada como colaborador.</summary>
        Task<bool> EsColaboradorAsync(int idEntidad);
        /// <summary>Marca el colaborador para ser insertado. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Colaborador colaborador);
        /// <summary>
        /// Trae el colaborador (con su entidad asociada) de una entidad, trackeado por EF
        /// para poder modificarlo. Devuelve null si esa entidad no es colaborador.
        /// </summary>
        Task<Colaborador?> ObtenerConEntidadPorEntidadAsync(int idEntidad);

    }
}
