using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;

namespace Nichos.Infrastructure.Repositories
{
    public class NichoRepository : INichoRepository
    {
        private readonly SociosDbContext _context;

        public NichoRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<List<NichoListadoDto>> BuscarNichoAsync(NichoFiltroDto filtro)
        {
            var query =
                from n in _context.Nichos
                join e in _context.Entidades
                    on n.Id_Entidad equals e.Id_Entidad into ne
                from e in ne.DefaultIfEmpty()
                select new { n, e };

            // 🔎 Filtro por nombre y/o apellido
            if (!string.IsNullOrEmpty(filtro.Busqueda))
            {
                query = query.Where(x => x.e != null &&
                    (x.e.Nombre.Contains(filtro.Busqueda) ||
                    x.e.Apellido.Contains(filtro.Busqueda)));
            }

            return await query.Select(x => new NichoListadoDto
            {
                Id_Nicho = x.n.Id_Nicho,
                Id_Entidad = x.n.Id_Entidad,
                EntidadNombre = x.e != null
                    ? x.e.Nombre + " " + x.e.Apellido
                    : "Sin Asignar",
                Sector = x.n.Sector,
                NroNicho = x.n.NroNicho,
                ValorNicho = x.n.ValorNicho,
                ValorLapida = x.n.ValorLapida,
                ConLapida = x.n.ConLapida,
                Ocupado = x.n.Ocupado,
                Cuotas = x.n.Cuotas,
                InteresMensual = x.n.InteresMensual
            }).ToListAsync();
        }
    }
}
