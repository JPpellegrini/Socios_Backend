using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class MetodoPagoRepository
    {
        private readonly SociosDbContext _context;

        public MetodoPagoRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
