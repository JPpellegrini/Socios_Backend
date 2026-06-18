using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CuotaRepository
    {
        private readonly SociosDbContext _context;

        public CuotaRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
