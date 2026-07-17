using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class TipoEntidadRepository : ITipoEntidadRepository
    {
        private readonly SociosDbContext _context;

        public TipoEntidadRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<int?> ObtenerIdPorNombreAsync(string nombre)
        {
            return await _context.TiposEntidad
                .Where(t => t.NombreTipoEntidad == nombre)
                .Select(t => (int?)t.Id_Tipo)
                .FirstOrDefaultAsync();
        }
    }
}
