using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CierreContableRepository
    {
        private readonly SociosDbContext _context;

        public CierreContableRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
