using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class DetalleMovimientoRepository : IDetalleMovimientoRepository
    {
        private readonly SociosDbContext _context;

        public DetalleMovimientoRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<DetalleMovimiento?> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.DetalleMovimiento
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.NombreDetalleMovimiento == nombre);
        }
    }
}
