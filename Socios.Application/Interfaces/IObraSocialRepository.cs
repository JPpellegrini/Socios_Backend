using Socios.Application.DTOs;
using Socios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface IObraSocialRepository
    {
        Task<IEnumerable<ObraSocial>> GetAllAsync();
        Task<ObraSocial?> GetByIdAsync(int id);
        Task<List<ObraSocialListadoDto>> BuscarAsync(ObraSocialFiltroDto filtro);
    }
}