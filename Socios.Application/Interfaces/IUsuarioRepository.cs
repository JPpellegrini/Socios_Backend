using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetUsuarioAsync(string usuarioNombre, string password);
    }
}
