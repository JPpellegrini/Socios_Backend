using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class ObraSocialRepository
    {
        private readonly SociosDbContext _context;

        public ObraSocialRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
