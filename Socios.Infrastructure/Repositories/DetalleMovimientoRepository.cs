using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class DetalleMovimientoRepository
    {
        private readonly SociosDbContext _context;

        public DetalleMovimientoRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
