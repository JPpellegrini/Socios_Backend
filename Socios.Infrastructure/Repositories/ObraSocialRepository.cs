using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class ObraSocialRepository : IObraSocialRepository
    {
        private readonly SociosDbContext _context;

        public ObraSocialRepository(SociosDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ObraSocial>> GetAllAsync()
        {
            return await _context.ObraSocial.ToListAsync();
        }

        public async Task<ObraSocial?> GetByIdAsync(int id)
        {
            return await _context.ObraSocial.FindAsync(id);
        }
        public async Task<List<ObraSocialListadoDto>> BuscarAsync(ObraSocialFiltroDto filtro)
        {
            if (string.IsNullOrEmpty(filtro.Busqueda))
                return new List<ObraSocialListadoDto>();

            return await _context.ObraSocial
                .Where(c => EF.Functions.Like(c.NombreObraSocial.ToUpper(), $"%{filtro.Busqueda.Trim().ToUpper()}%"))
                .Select(c => new ObraSocialListadoDto
                {
                    Id_ObraSocial = c.Id_ObraSocial,
                    NombreObraSocial = c.NombreObraSocial
                })
                .ToListAsync();
        }
    }
}
