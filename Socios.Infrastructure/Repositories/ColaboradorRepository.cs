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
    }
}
