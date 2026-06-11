using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CajaRepository
    {
        private readonly SociosDbContext _context;

        public CajaRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
