using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class TipoPlanRepository : ITipoPlanRepository
    {
        private readonly SociosDbContext _context;

        public TipoPlanRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Socios.Domain.Entities.TipoPlan?> ObtenerPorTipoCuotaAsync(int idTipoCuota)
        {
            // Trackeado (sin AsNoTracking) para que el caso de uso pueda modificar la edad tope.
            return await _context.TiposPlan.FirstOrDefaultAsync(tp => tp.Id_TipoCuota == idTipoCuota);
        }
    }
}
