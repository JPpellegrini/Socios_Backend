using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class PrestacionRepository : IPrestacionRepository
    {
        private readonly SociosDbContext _context;

        public PrestacionRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteAsync(int idPrestacion)
        {
            return await _context.Prestacion.AnyAsync(p => p.Id_Prestacion == idPrestacion);
        }

        public async Task<List<PrestacionDto>> ListarAsync()
        {
            return await _context.Prestacion.AsNoTracking()
                .OrderBy(p => p.NombrePrestacion)
                .Select(p => new PrestacionDto { IdPrestacion = p.Id_Prestacion, Nombre = p.NombrePrestacion })
                .ToListAsync();
        }

        public void Agregar(Prestacion prestacion)
        {
            // Solo marca la prestación para insertar. El guardado real lo dispara la unidad de trabajo.
            _context.Prestacion.Add(prestacion);
        }
    }
}
