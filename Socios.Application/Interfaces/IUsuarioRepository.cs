using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByUsuarioNombreAsync(string usuarioNombre);
    }
}
