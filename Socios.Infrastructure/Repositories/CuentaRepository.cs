using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CuentaRepository
    {
        private readonly SociosDbContext _context;

        public CuentaRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
