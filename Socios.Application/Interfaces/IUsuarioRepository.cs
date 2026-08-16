using Socios.Domain.Entities;
using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByUsuarioNombreAsync(string usuarioNombre);
        Task<List<UsuarioListadoDto>> BuscarUsuarioAsync(UsuarioFiltroDto filtro);
    }
}
