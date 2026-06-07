using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class PrestacionRepository
    {
        private readonly SociosDbContext _context;

        public PrestacionRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
