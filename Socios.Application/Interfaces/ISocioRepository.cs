using Socios.Application.DTOs;
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

        Task<List<SocioListadoDto>> BuscarAsync(SocioFiltroDto filtro);

        /// <summary>
        /// Da de alta un socio en cascada (Entidad + Socio + EntidadTipo + Contactos)
        /// dentro de una única transacción. Devuelve el Id_Socio generado.
        /// </summary>
        Task<int> CrearAsync(SocioCrearDto dto);
    }
}
