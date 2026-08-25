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

            // 🔎 Búsqueda del socio: nombre, apellido o DNI. Si viene vacío, no filtra.
            // ILIKE = case-insensitive; el Trim ignora espacios al inicio/fin.
            if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
            {
                var termino = $"%{filtro.Busqueda.Trim()}%";
                query = query.Where(x => x.e != null &&
                    (EF.Functions.ILike(x.e.Nombre, termino) ||
                     EF.Functions.ILike(x.e.Apellido, termino) ||
                     EF.Functions.ILike(x.e.Dni, termino)));
            }

            // 🔎 Búsqueda del nicho: sector o número. Si viene vacío, no filtra.
            if (!string.IsNullOrWhiteSpace(filtro.SectorNumero))
            {
                var nicho = $"%{filtro.SectorNumero.Trim()}%";
                query = query.Where(x =>
                    EF.Functions.ILike(x.n.Sector, nicho) ||
                    EF.Functions.ILike(x.n.NroNicho, nicho));
            }

            // ☑ Ocupación: Ocupado se persiste como "SI"/"NO".
            // false = solo libres; true = todos (incluye ocupados, no filtra).
            if (!filtro.Ocupado)
            {
                query = query.Where(x => x.n.Ocupado == "NO");
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
