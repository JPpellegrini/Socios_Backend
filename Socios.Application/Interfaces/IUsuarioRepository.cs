using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task AddAsync(Usuario usuario);
    }
}
