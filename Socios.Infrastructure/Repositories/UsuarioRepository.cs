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

        public async Task<Usuario>CrearUsuarioAsync(UsuarioCrearDto dto)
        {
            // 1. Hashear la contraseña
            string passwordHasheada = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 2. Crear la entidad Usuario
            var usuario = new Usuario
            {
                UsuarioNombre = dto.UsuarioNombre.ToUpper().Trim(),
                Password = passwordHasheada,
                Estado = dto.Estado.ToUpper().Trim(),
                Id_Rol = dto.Id_Rol
            };

            // 3. Guardar en la base
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<Usuario?> DarDeBajaUsuarioAsync(int idUsuario)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id_Usuario == idUsuario);
            if (usuario == null) return null;

            usuario.Estado = "INACTIVO";
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<Usuario?> ModificarUsuarioAsync(int idUsuario, UsuarioModificarDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id_Usuario == idUsuario);

            if (usuario == null)
                return null;

            // Si viene nueva contraseña, la hasheamos
            if (!string.IsNullOrEmpty(dto.Password))
            {
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            // Si viene nuevo rol, lo actualizamos
            if (dto.Id_Rol.HasValue)
            {
                usuario.Id_Rol = dto.Id_Rol.Value;
            }

            // Si viene nuevo estado, lo actualizamos en mayúsculas
            if (!string.IsNullOrEmpty(dto.Estado))
            {
                usuario.Estado = dto.Estado.ToUpper().Trim();
            }

            await _context.SaveChangesAsync();
            return usuario;
        }
    }
}
