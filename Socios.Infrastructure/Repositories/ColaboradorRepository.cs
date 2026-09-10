using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class ColaboradorRepository : IColaboradorRepository
    {
        private readonly SociosDbContext _context;

        public ColaboradorRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Colaborador> AddAsync(Colaborador colaborador)
        {
            _context.Add(colaborador);
            await _context.SaveChangesAsync();
            return colaborador;
        }

        public async Task DeleteAsync(int idColaborador)
        {
            var item = await _context.Set<Colaborador>().FindAsync(idColaborador);
            if (item != null)
            {
                _context.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Colaborador>> GetAllAsync()
        {
            return await _context.Set<Colaborador>().Include(c => c.Prestacion).Include(c => c.Entidad).ToListAsync();
        }

        public async Task<Colaborador?> GetByIdAsync(int idColaborador)
        {
            return await _context.Set<Colaborador>().Include(c => c.Prestacion).Include(c => c.Entidad).FirstOrDefaultAsync(c => c.Id_Colaborador == idColaborador);
        }

        public async Task UpdateAsync(Colaborador colaborador)
        {
            _context.Update(colaborador);
            await _context.SaveChangesAsync();
        }

        public void AgregarEntidad(Entidad entidad) => _context.Entidades.Add(entidad);
        public void AgregarEntidadTipo(EntidadTipo entidadTipo) => _context.EntidadTipos.Add(entidadTipo);
        public void ActualizarEntidad(Entidad entidad) => _context.Entidades.Update(entidad);
        public void ActualizarEntidadTipo(EntidadTipo entidadTipo) => _context.EntidadTipos.Update(entidadTipo);
        public async Task<EntidadTipo?> ObtenerEntidadTipoAsync(int idEntidadTipo)
        {
            return await _context.EntidadTipos
                .Include(et => et.Entidad)
                .FirstOrDefaultAsync(et => et.Id_EntidadTipo == idEntidadTipo && et.Id_Tipo == 3);
        }

        public async Task<List<Entidad>> BuscarAsync(string? busqueda)
        {
            var query =
                from e in _context.Entidades
                    .Include(e => e.Ciudad)
                    .Include(e => e.Contactos) // 👈 importante: incluir contactos
                join et in _context.EntidadTipos on e.Id_Entidad equals et.Id_Entidad
                where et.Id_Tipo == 3 && et.Estado == "ACTIVO"
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

        public async Task RegistrarBajaAsync(EntidadBaja baja)
        {
            await _context.EntidadBajas.AddAsync(baja);
        }

    }
}
