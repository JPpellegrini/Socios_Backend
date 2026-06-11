using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class EntidadBajaRepository : IEntidadBajaRepository
    {
        private readonly SociosDbContext _context;

        public EntidadBajaRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<EntidadBaja> AddAsync(EntidadBaja entidadBaja)
        {
            _context.Add(entidadBaja);
            await _context.SaveChangesAsync();
            return entidadBaja;
        }

        public async Task DeleteAsync(int idBaja)
        {
            var baja = await _context.Set<EntidadBaja>().FindAsync(idBaja);
            if (baja != null)
            {
                _context.Remove(baja);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<EntidadBaja>> GetAllAsync()
        {
            return await _context.Set<EntidadBaja>().ToListAsync();
        }

        public async Task<EntidadBaja?> GetByIdAsync(int idBaja)
        {
            return await _context.Set<EntidadBaja>().FindAsync(idBaja);
        }

        public async Task UpdateAsync(EntidadBaja entidadBaja)
        {
            _context.Update(entidadBaja);
            await _context.SaveChangesAsync();
        }
    }
}
