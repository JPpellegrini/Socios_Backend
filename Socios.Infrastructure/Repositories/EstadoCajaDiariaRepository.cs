using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class EstadoCajaDiariaRepository
    {
        private readonly SociosDbContext _context;

        public EstadoCajaDiariaRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
