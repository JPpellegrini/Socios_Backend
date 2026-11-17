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

        public async Task<(string Tipo, string Estado)?> ObtenerTipoYEstadoAsync(int idPrestacion)
        {
            var datos = await _context.Prestacion.AsNoTracking()
                .Where(p => p.Id_Prestacion == idPrestacion)
                .Select(p => new { p.Tipo, p.Estado })
                .FirstOrDefaultAsync();

            return datos is null ? null : (datos.Tipo, datos.Estado);
        }

        public async Task<List<PrestacionDto>> ListarAsync(string? tipo = null, string? estado = null)
        {
            var consulta = _context.Prestacion.AsNoTracking();
            if (tipo is not null)
                consulta = consulta.Where(p => p.Tipo == tipo);
            if (estado is not null)
                consulta = consulta.Where(p => p.Estado == estado);

            return await consulta
                .OrderBy(p => p.NombrePrestacion)
                .Select(p => new PrestacionDto
                {
                    IdPrestacion = p.Id_Prestacion,
                    Nombre = p.NombrePrestacion,
                    Tipo = p.Tipo,
                    Estado = p.Estado
                })
                .ToListAsync();
        }

        public async Task<Prestacion?> ObtenerAsync(int idPrestacion)
        {
            return await _context.Prestacion.FirstOrDefaultAsync(p => p.Id_Prestacion == idPrestacion);
        }

        public void Agregar(Prestacion prestacion)
        {
            // Solo marca la prestación para insertar. El guardado real lo dispara la unidad de trabajo.
            _context.Prestacion.Add(prestacion);
        }
    }
}
