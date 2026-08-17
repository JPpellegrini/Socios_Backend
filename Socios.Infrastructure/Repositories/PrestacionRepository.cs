using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class PrestacionRepository : IPrestacionRepository
    {
        private readonly SociosDbContext _context;

        public PrestacionRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteAsync(int idPrestacion)
        {
            return await _context.Prestacion.AnyAsync(p => p.Id_Prestacion == idPrestacion);
        }
    }
}
