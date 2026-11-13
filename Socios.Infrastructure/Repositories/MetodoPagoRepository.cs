using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class MetodoPagoRepository : IMetodoPagoRepository
    {
        private readonly SociosDbContext _context;

        public MetodoPagoRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<MetodoPago?> ObtenerPorIdAsync(int idMetodoPago)
        {
            return await _context.MetodoPago
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id_MetodoPago == idMetodoPago);
        }

        public async Task<MetodoPago?> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.MetodoPago
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.NombreMetodoPago == nombre);
        }
    }
}
