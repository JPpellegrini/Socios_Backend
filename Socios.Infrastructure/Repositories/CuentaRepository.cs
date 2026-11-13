using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CuentaRepository : ICuentaRepository
    {
        private readonly SociosDbContext _context;

        public CuentaRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Cuenta?> ObtenerPorIdAsync(int idCuenta)
        {
            return await _context.Cuenta
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id_Cuenta == idCuenta);
        }

        public async Task<Cuenta?> ObtenerPorNombreAsync(string nombreCuenta)
        {
            return await _context.Cuenta
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.NombreCuenta == nombreCuenta);
        }

        public async Task<List<Cuenta>> ListarActivasAsync()
        {
            return await _context.Cuenta
                .AsNoTracking()
                .Where(c => c.Estado == "ACTIVA")
                .OrderBy(c => c.NombreCuenta)
                .ToListAsync();
        }
    }
}
