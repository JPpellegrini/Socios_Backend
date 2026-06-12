using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class TipoCuotaRepository 
    {
        private readonly SociosDbContext _context;

        public TipoCuotaRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
