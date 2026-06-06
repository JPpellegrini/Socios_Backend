using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CiudadRepository : ICiudadRepository
    {
        private readonly SociosDbContext _context;

        public CiudadRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ciudad>> GetAllAsync()
        {
            return await _context.Ciudades.ToListAsync();
        }

        public async Task<Ciudad?> GetByIdAsync(int id)
        {
            return await _context.Ciudades.FindAsync(id);
        }
    }
}
