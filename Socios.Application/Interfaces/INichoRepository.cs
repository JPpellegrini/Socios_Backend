using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    public interface INichoRepository
    {
        Task<List<NichoListadoDto>> BuscarNichoAsync(NichoFiltroDto filtro);
    }
}
