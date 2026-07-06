using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class EntidadRepository : IEntidadRepository
    {
        private readonly SociosDbContext _context;

        public EntidadRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Entidad> AddAsync(Entidad entidad)
        {
            _context.Add(entidad);
            await _context.SaveChangesAsync();
            return entidad;
        }

        public async Task DeleteAsync(int id)
        {
            var entidad = await _context.Set<Entidad>().FindAsync(id);
            if (entidad != null)
            {
                _context.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Entidad>> GetAllAsync()
        {
            return await _context.Set<Entidad>().Include(e => e.Ciudad).ToListAsync();
        }

        public async Task<Entidad?> GetByIdAsync(int id)
        {
            return await _context.Set<Entidad>().Include(e => e.Ciudad).FirstOrDefaultAsync(e => e.Id_Entidad == id);
        }

        public async Task UpdateAsync(Entidad entidad)
        {
            _context.Update(entidad);
            await _context.SaveChangesAsync();
        }

        public async Task<EntidadDto?> BuscarAsync(EntidadFiltroDto filtro)
        {
            var query =
                from e in _context.Entidades
                join c in _context.Ciudades on e.Id_Ciudad equals c.Id_Ciudad
                select new { e, c };

            var busqueda = filtro.Dni?.Trim();

            if (!string.IsNullOrEmpty(busqueda))
                query = query.Where(x => (x.e.Dni == busqueda));
            
            return await query.Select(x => new EntidadDto
            {
                Id_Entidad = x.e.Id_Entidad,
                CuitCuil = x.e.CuitCuil,
                Nombre = x.e.Nombre,
                Apellido = x.e.Apellido,
                RazonSocial = x.e.RazonSocial,
                Sexo = x.e.Sexo,
                Nacimiento = x.e.Nacimiento,
                Ciudad = new Ciudad
                {
                    Id_Ciudad = x.c.Id_Ciudad,
                    Nombre = x.c.Nombre
                },
                Calle = x.e.Calle,
                Altura = x.e.Altura,
                Observacion = x.e.Observacion
            }).FirstOrDefaultAsync();
        }
    }
}
