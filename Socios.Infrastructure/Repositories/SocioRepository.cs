using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class SocioRepository : ISocioRepository
    {
        private readonly SociosDbContext _context;

        public SocioRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Socio> AddAsync(Socio socio)
        {
            _context.Add(socio);
            await _context.SaveChangesAsync();
            return socio;
        }

        public async Task DeleteAsync(int idEntidad)
        {
            var socio = await _context.Set<Socio>().FindAsync(idEntidad);
            if (socio != null)
            {
                _context.Remove(socio);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Socio>> GetAllAsync()
        {
            return await _context.Set<Socio>().Include(s => s.ObraSocial).Include(s => s.Entidad).ToListAsync();
        }

        public async Task<Socio?> GetByIdAsync(int idEntidad)
        {
            return await _context.Set<Socio>().Include(s => s.ObraSocial).Include(s => s.Entidad).FirstOrDefaultAsync(s => s.Id_Entidad == idEntidad);
        }

        public async Task UpdateAsync(Socio socio)
        {
            _context.Update(socio);
            await _context.SaveChangesAsync();
        }
    }
}
