using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CuotaRepository : ICuotaRepository
    {
        private readonly SociosDbContext _context;

        public CuotaRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<List<CuotaListadoDto>> BuscarAsync(CuotaFiltroDto filtro)
        {
            // La cuota apunta a Id_Entidad; para mostrar el Id del socio y sus datos
            // (Nombre/Apellido/DNI) cruzamos por esa entidad. Join interno con socios:
            // solo se listan cuotas de entidades que son socios (todas las cuotas lo son).
            var query =
                from c in _context.Cuotas
                join s in _context.Socios on c.Id_Entidad equals s.Id_Entidad
                join e in _context.Entidades on c.Id_Entidad equals e.Id_Entidad
                join tc in _context.TiposCuotas on c.Id_TipoCuota equals tc.Id_TipoCuota
                select new { c, s, e, tc };

            // Campo único de socio: matchea por Nombre, Apellido o DNI (ILIKE = case-insensitive).
            var busqueda = filtro.Busqueda?.Trim();
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = $"%{busqueda}%";
                query = query.Where(x =>
                    (x.e.Nombre != null && EF.Functions.ILike(x.e.Nombre, patron)) ||
                    (x.e.Apellido != null && EF.Functions.ILike(x.e.Apellido, patron)) ||
                    (x.e.Dni != null && EF.Functions.ILike(x.e.Dni, patron)));
            }

            // Filtro por tipo de cuota (concepto). El DTO valida los valores permitidos
            // ([AllowedValues]); acá "TODOS" o vacío no filtra. El resto matchea directo
            // contra el concepto guardado (SOCIO/SEPELIO/NICHO).
            var tipoCuota = filtro.TipoCuota?.Trim();
            if (!string.IsNullOrWhiteSpace(tipoCuota) && tipoCuota != "TODOS")
            {
                query = query.Where(x => x.tc.Concepto == tipoCuota);
            }

            // Filtro por estado. El DTO valida los valores permitidos ([AllowedValues]);
            // acá "TODAS" o vacío no filtra. El resto matchea directo contra el literal
            // guardado en la columna Estado (PAGADO/PENDIENTE/VENCIDA/ANULADA).
            var estado = filtro.Estado?.Trim();
            if (!string.IsNullOrWhiteSpace(estado) && estado != "TODAS")
            {
                query = query.Where(x => x.c.Estado == estado);
            }

            return await query
                .OrderBy(x => x.e.Apellido)
                .ThenBy(x => x.e.Nombre)
                .ThenByDescending(x => x.c.Anio_Periodo)
                .ThenByDescending(x => x.c.Mes_Periodo)
                .Select(x => new CuotaListadoDto
                {
                    IdCuota = x.c.Id_Deuda,
                    IdSocio = x.s.Id_Socio,
                    Nombre = x.e.Nombre,
                    Apellido = x.e.Apellido,
                    Documento = x.e.Dni,
                    TipoCuota = x.tc.Concepto,
                    Mes = x.c.Mes_Periodo,
                    Anio = x.c.Anio_Periodo,
                    Monto = x.c.Monto,
                    FechaVencimiento = x.c.FechaVencimiento,
                    Estado = x.c.Estado
                })
                .ToListAsync();
        }
    }
}
