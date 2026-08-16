using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;

namespace Socios.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SociosDbContext _context;

        public UsuarioRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByUsuarioNombreAsync(string usuarioNombre)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(x => x.UsuarioNombre == usuarioNombre);
        }

        public async Task<List<UsuarioListadoDto>> BuscarUsuarioAsync(UsuarioFiltroDto filtro)
        {
            var query =
                from u in _context.Usuarios
                join r in _context.Roles on u.Id_Rol equals r.Id_Rol
                where (string.IsNullOrEmpty(filtro.Usuario)
                       || u.UsuarioNombre.ToLower().Contains(filtro.Usuario.Trim().ToLower()))
                   && (string.IsNullOrEmpty(filtro.Rol)
                       || r.RolNombre.ToLower().Contains(filtro.Rol.Trim().ToLower()))
                select new UsuarioListadoDto
                {
                    Id_Usuario = u.Id_Usuario,
                    UsuarioNombre = u.UsuarioNombre,
                    Estado = u.Estado,
                    RolNombre = r.RolNombre,
                    Descripcion = r.Descripcion
                };

            return await query.ToListAsync();
        }
    }
}
