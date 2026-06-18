using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class MovimientoPrestacionRepository
    {
        private readonly SociosDbContext _context;

        public MovimientoPrestacionRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
