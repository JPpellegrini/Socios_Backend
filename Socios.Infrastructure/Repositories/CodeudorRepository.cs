using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class CodeudorRepository : ICodeudorRepository
    {
        private readonly SociosDbContext _context;

        public CodeudorRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Codeudor> AddAsync(Codeudor codeudor)
        {
            _context.Add(codeudor);
            await _context.SaveChangesAsync();
            return codeudor;
        }

        /// <summary>Marca la relación socio↔codeudor para ser insertada. NO guarda: eso lo hace la unidad de trabajo.</summary>
        public void Agregar(Codeudor codeudor)
        {
            _context.Add(codeudor);
        }

        public async Task DeleteAsync(int idEntidad, int idEntidadCodeudor)
        {
            var key = new { Id_Entidad_Codeudor = idEntidadCodeudor, Id_Entidad = idEntidad };
            var codeudor = await _context.Set<Codeudor>().FindAsync(idEntidadCodeudor, idEntidad);
            if (codeudor != null)
            {
                _context.Remove(codeudor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Codeudor>> GetAllAsync()
        {
            return await _context.Set<Codeudor>().ToListAsync();
        }

        public async Task<Codeudor?> GetByIdAsync(int idEntidad, int idEntidadCodeudor)
        {
            return await _context.Set<Codeudor>().FindAsync(idEntidadCodeudor, idEntidad);
        }

        public async Task UpdateAsync(Codeudor codeudor)
        {
            _context.Update(codeudor);
            await _context.SaveChangesAsync();
        }
    }
}
