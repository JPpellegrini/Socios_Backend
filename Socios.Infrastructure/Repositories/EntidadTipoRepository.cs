using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class EntidadTipoRepository : IEntidadTipoRepository
    {
        private readonly SociosDbContext _context;

        public EntidadTipoRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<EntidadTipo> AddAsync(EntidadTipo entidadTipo)
        {
            _context.Add(entidadTipo);
            await _context.SaveChangesAsync();
            return entidadTipo;
        }

        public async Task DeleteAsync(int idEntidad, int idTipo)
        {
            var entidadTipo = await _context.Set<EntidadTipo>().FindAsync(idEntidad, idTipo);
            if (entidadTipo != null)
            {
                _context.Remove(entidadTipo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<EntidadTipo>> GetAllAsync()
        {
            return await _context.Set<EntidadTipo>().ToListAsync();
        }

        public async Task<EntidadTipo?> GetByIdAsync(int idEntidad, int idTipo)
        {
            return await _context.Set<EntidadTipo>().FindAsync(idEntidad, idTipo);
        }

        public async Task UpdateAsync(EntidadTipo entidadTipo)
        {
            _context.Update(entidadTipo);
            await _context.SaveChangesAsync();
        }

        public void Agregar(EntidadTipo entidadTipo)
        {
            // Solo marca el registro para insertar. El guardado real lo dispara la unidad de trabajo.
            _context.EntidadTipos.Add(entidadTipo);
        }

        public async Task<EntidadTipo?> ObtenerPorEntidadYTipoAsync(int idEntidad, int idTipo)
        {
            return await _context.EntidadTipos.FirstOrDefaultAsync(et => et.Id_Entidad == idEntidad && et.Id_Tipo == idTipo);
        }
    }
}
