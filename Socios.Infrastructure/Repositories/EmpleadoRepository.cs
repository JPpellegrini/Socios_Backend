using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly SociosDbContext _context;

        public EmpleadoRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Entidad?> ObtenerPorDniAsync(string dni)
        {
            return await _context.Entidades.FirstOrDefaultAsync(e => e.Dni == dni);
        }

        public async Task<bool> EsEmpleadoAsync(int idEntidad)
        {
            return await _context.EntidadTipos
                .AnyAsync(et => et.Id_Entidad == idEntidad && et.Id_Tipo == 4 && et.Estado == "ACTIVO");
        }

        public async Task<Entidad?> ObtenerPorIdAsync(int idEntidad)
        {
            return await _context.Entidades
                .Include(e => e.Ciudad)
                .FirstOrDefaultAsync(e => e.Id_Entidad == idEntidad);
        }

        public async Task<EntidadTipo?> ObtenerEntidadTipoAsync(int idEntidadTipo)
        {
            return await _context.EntidadTipos
                .Include(et => et.Entidad)
                .FirstOrDefaultAsync(et => et.Id_EntidadTipo == idEntidadTipo && et.Id_Tipo == 4);
        }

        public void AgregarEntidad(Entidad entidad) => _context.Entidades.Add(entidad);
        public void AgregarEntidadTipo(EntidadTipo entidadTipo) => _context.EntidadTipos.Add(entidadTipo);
        public void ActualizarEntidad(Entidad entidad) => _context.Entidades.Update(entidad);
        public void ActualizarEntidadTipo(EntidadTipo entidadTipo) => _context.EntidadTipos.Update(entidadTipo);

        public async Task<List<Entidad>> BuscarAsync(string? busqueda)
        {
            var query =
                from e in _context.Entidades
                    .Include(e => e.Ciudad)
                    .Include(e => e.Contactos) // 👈 importante: incluir contactos
                join et in _context.EntidadTipos on e.Id_Entidad equals et.Id_Entidad
                where et.Id_Tipo == 4 && et.Estado == "ACTIVO"
                select e;

            if (!string.IsNullOrEmpty(busqueda))
            {
                var filtro = busqueda.ToLower();
                query = query.Where(e =>
                    e.Dni.ToLower().Contains(filtro) ||
                    e.Nombre.ToLower().Contains(filtro) ||
                    e.Apellido.ToLower().Contains(filtro));
            }

            return await query.ToListAsync();
        }
    }
}