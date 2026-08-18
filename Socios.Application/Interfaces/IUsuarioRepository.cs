using Socios.Domain.Entities;
using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByUsuarioNombreAsync(string usuarioNombre);
        Task<List<UsuarioListadoDto>> BuscarUsuarioAsync(UsuarioFiltroDto filtro);
        Task<Usuario> CrearUsuarioAsync(UsuarioCrearDto dto);
        Task<Usuario?> DarDeBajaUsuarioAsync(int idUsuario);
        Task<Usuario?> ModificarUsuarioAsync(int idUsuario, UsuarioModificarDto dto);
    }
}
