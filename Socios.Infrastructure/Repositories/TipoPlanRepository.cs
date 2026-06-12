using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class TipoPlanRepository 
    {
        private readonly SociosDbContext _context;

        public TipoPlanRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
