using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class NichoRepository
    {
        private readonly SociosDbContext _context;

        public NichoRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
