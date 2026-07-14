using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class ContactoRepository : IContactoRepository
    {
        private readonly SociosDbContext _context;

        public ContactoRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Contacto> AddAsync(Contacto contacto)
        {
            _context.Add(contacto);
            await _context.SaveChangesAsync();
            return contacto;
        }

        public async Task DeleteAsync(int idContacto)
        {
            var item = await _context.Set<Contacto>().FindAsync(idContacto);
            if (item != null)
            {
                _context.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Contacto>> GetAllAsync()
        {
            return await _context.Set<Contacto>().Include(c => c.Entidad).ToListAsync();
        }

        public async Task<Contacto?> GetByIdAsync(int idContacto)
        {
            return await _context.Set<Contacto>().Include(c => c.Entidad).FirstOrDefaultAsync(c => c.Id_Contacto == idContacto);
        }

        public async Task UpdateAsync(Contacto contacto)
        {
            _context.Update(contacto);
            await _context.SaveChangesAsync();
        }

        public void Agregar(Contacto contacto)
        {
            // Solo marca el contacto para insertar. El guardado real lo dispara la unidad de trabajo.
            _context.Contactos.Add(contacto);
        }
    }
}
