using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class EntidadRepository : IEntidadRepository
    {
        private readonly SociosDbContext _context;

        public EntidadRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Entidad> AddAsync(Entidad entidad)
        {
            _context.Add(entidad);
            await _context.SaveChangesAsync();
            return entidad;
        }

        public async Task DeleteAsync(int id)
        {
            var entidad = await _context.Set<Entidad>().FindAsync(id);
            if (entidad != null)
            {
                _context.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Entidad>> GetAllAsync()
        {
            return await _context.Set<Entidad>().Include(e => e.Ciudad).ToListAsync();
        }

        public async Task<Entidad?> GetByIdAsync(int id)
        {
            return await _context.Set<Entidad>().Include(e => e.Ciudad).FirstOrDefaultAsync(e => e.Id_Entidad == id);
        }

        public async Task UpdateAsync(Entidad entidad)
        {
            _context.Update(entidad);
            await _context.SaveChangesAsync();
        }
    }
}
