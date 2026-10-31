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

        public async Task<List<Socios.Domain.Entities.TipoPlan>> ObtenerSepelioAsync()
        {
            // Solo lectura; se incluye el tipo de cuota para leer su importe y concepto.
            return await _context.TiposPlan
                .AsNoTracking()
                .Include(tp => tp.TipoCuota)
                .Where(tp => tp.TipoCuota.Concepto == "SEPELIO")
                .ToListAsync();
        }
    }
}
