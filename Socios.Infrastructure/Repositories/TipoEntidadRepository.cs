using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class TipoEntidadRepository 
    {
        private readonly SociosDbContext _context;

        public TipoEntidadRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
