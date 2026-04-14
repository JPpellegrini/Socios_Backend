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

        public async Task<Usuario?> GetUsuarioAsync(string usuarioNombre, string password)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.UsuarioNombre == usuarioNombre);

            if (usuario == null) 
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, usuario.Password)) 
                return null;

            return usuario;
        }
    }
}
