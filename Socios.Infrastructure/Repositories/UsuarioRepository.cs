using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Socios.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SociosDbContext _context;

        public UsuarioRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByEmailAsync(string usuarioNombre)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(x => x.UsuarioNombre == usuarioNombre);
        }
    }
}
