using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CiudadRepository : ICiudadRepository
    {
        private readonly SociosDbContext _context;

        public CiudadRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ciudad>> GetAllAsync()
        {
            return await _context.Ciudades.ToListAsync();
        }

        public async Task<Ciudad?> GetByIdAsync(int id)
        {
            return await _context.Ciudades.FindAsync(id);
        }
        public async Task<List<CiudadListadoDto>> BuscarAsync(CiudadFiltroDto filtro)
        {
            if (string.IsNullOrEmpty(filtro.Busqueda))
                return new List<CiudadListadoDto>();

            return await _context.Ciudades
                .Where(c => EF.Functions.Like(c.Nombre, $"%{filtro.Busqueda.Trim()}%"))
                .Select(c => new CiudadListadoDto
                {
                    Id_Ciudad = c.Id_Ciudad,
                    Nombre = c.Nombre
                })
                .ToListAsync();
        }
    }
}
