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

        Task<int> CrearAsync(SocioCrearDto dto);

        Task DarDeBajaAsync(int idSocio, SocioBajaDto dto);

        /// <summary>Indica si la entidad indicada ya está registrada como socio.</summary>
        Task<bool> EsSocioAsync(int idEntidad);

        /// <summary>Marca el socio para ser insertado. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Socio socio);
    }
}
