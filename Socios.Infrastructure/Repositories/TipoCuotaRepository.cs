using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class TipoCuotaRepository : ITipoCuotaRepository
    {
        private readonly SociosDbContext _context;

        public TipoCuotaRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<List<ConfiguracionCuotaListadoDto>> BuscarConfiguracionAsync(ConfiguracionCuotaFiltroDto filtro)
        {
            // LEFT JOIN tipo_cuotas -> tipo_planes: un tipo de cuota puede no tener plan
            // asociado (SOCIO, NICHO), y en ese caso la fila igual tiene que aparecer.
            var query =
                from tc in _context.TiposCuotas
                join tp in _context.TiposPlan
                    on tc.Id_TipoCuota equals tp.Id_TipoCuota into tcp
                from tp in tcp.DefaultIfEmpty()
                select new { tc, tp };

            // 🔎 Filtro por concepto. Vacío o "todos" no filtra (incluye NICHO).
            var concepto = filtro.Concepto?.Trim();
            if (!string.IsNullOrWhiteSpace(concepto) &&
                !concepto.Equals("todos", StringComparison.OrdinalIgnoreCase))
            {
                // Concepto se persiste en mayúsculas (SOCIO / SEPELIO). Normalizamos el
                // input a mayúsculas de forma independiente de la cultura del servidor,
                // así "sepelIO" / "SoCio" matchean igual.
                var conceptoUpper = concepto.ToUpperInvariant();
                query = query.Where(x => x.tc.Concepto == conceptoUpper);
            }

            // Traemos las filas crudas y armamos la columna "Edad" en memoria: la fila
            // "MAS DE n" no guarda el número de tope, lo toma de su fila hermana (mismo
            // concepto + plan), así que necesitamos tener todo el conjunto a mano.
            var filas = await query
                .Select(x => new
                {
                    x.tc.Id_TipoCuota,
                    x.tc.Concepto,
                    x.tc.Importe,
                    x.tc.Tiene_EdadTope,
                    x.tc.Fecha_ultimamodif,
                    EdadTope = x.tp != null ? x.tp.EdadTope : (int?)null,
                    TipoPlan = x.tp != null ? x.tp.Tipo_Plan : null
                })
                .ToListAsync();

            // Tope de edad por (Concepto, Plan). El número (ej. 65) está cargado en la fila
            // "HASTA"; la fila "MAS DE" lo reutiliza de acá.
            var topesPorPlan = filas
                .Where(f => f.EdadTope != null && f.TipoPlan != null)
                .GroupBy(f => (f.Concepto, f.TipoPlan))
                .ToDictionary(g => g.Key, g => g.Max(f => f.EdadTope));

            return filas
                .Select(f =>
                {
                    string edad;
                    if (f.TipoPlan == null)
                    {
                        // Sin plan (SOCIO, NICHO): la edad no entra en juego.
                        edad = "SIN TOPE";
                    }
                    else
                    {
                        // El número de tope sale de la propia fila o de su hermana.
                        topesPorPlan.TryGetValue((f.Concepto, f.TipoPlan), out var topeHermana);
                        var tope = f.EdadTope ?? topeHermana;

                        // Tiene_EdadTope distingue la cuota "hasta el tope" de la que lo supera.
                        edad = tope == null
                            ? "SIN TOPE"
                            : f.Tiene_EdadTope ? $"HASTA {tope}" : $"MAS DE {tope}";
                    }

                    return new ConfiguracionCuotaListadoDto
                    {
                        Id_TipoCuota = f.Id_TipoCuota,
                        Concepto = f.Concepto,
                        Importe = f.Importe,
                        Edad = edad,
                        Plan = f.TipoPlan ?? "-",
                        UltimaModificacion = f.Fecha_ultimamodif
                    };
                })
                .OrderBy(r => r.Id_TipoCuota)
                .ToList();
        }
    }
}
