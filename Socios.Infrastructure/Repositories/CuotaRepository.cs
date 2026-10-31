using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
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

        public async Task<List<CuotaClaveDto>> ObtenerClavesPeriodoAsync(int idEntidad, int desdeClave, int hastaClave)
        {
            // Clave comparable año*100 + mes, para filtrar el rango (año, mes) con una
            // sola condición ordenable. Se trae concepto (vía tipo de cuota) + mes + año.
            // Las anuladas no cuentan: anular una cuota permite volver a generarla.
            return await (
                from c in _context.Cuotas
                join tc in _context.TiposCuotas on c.Id_TipoCuota equals tc.Id_TipoCuota
                where c.Id_Entidad == idEntidad
                    && c.Estado != "ANULADA"
                    && (c.Anio_Periodo * 100 + c.Mes_Periodo) >= desdeClave
                    && (c.Anio_Periodo * 100 + c.Mes_Periodo) <= hastaClave
                select new CuotaClaveDto
                {
                    IdEntidad = c.Id_Entidad,
                    Concepto = tc.Concepto,
                    Mes = c.Mes_Periodo,
                    Anio = c.Anio_Periodo
                }
            ).ToListAsync();
        }

        public async Task<List<CuotaClaveDto>> ObtenerClavesMesAsync(int mes, int anio)
        {
            // Una sola consulta para todos los socios del mes (evita una consulta por socio).
            // Las anuladas no cuentan: anular una cuota permite volver a generarla.
            return await (
                from c in _context.Cuotas
                join tc in _context.TiposCuotas on c.Id_TipoCuota equals tc.Id_TipoCuota
                where c.Mes_Periodo == mes
                    && c.Anio_Periodo == anio
                    && c.Estado != "ANULADA"
                select new CuotaClaveDto
                {
                    IdEntidad = c.Id_Entidad,
                    Concepto = tc.Concepto,
                    Mes = c.Mes_Periodo,
                    Anio = c.Anio_Periodo
                }
            ).ToListAsync();
        }

        public void Agregar(Cuota cuota)
        {
            // Solo marca la cuota para insertar. El guardado real lo dispara la unidad de trabajo.
            _context.Cuotas.Add(cuota);
        }

        public async Task<Cuota?> ObtenerPorIdAsync(int idDeuda)
        {
            // Trackeado (sin AsNoTracking) para que el caso de uso pueda anularla y la
            // unidad de trabajo confirme el cambio. Se incluye el tipo de cuota para
            // conocer el concepto (NICHO dispara la anulación en cascada).
            return await _context.Cuotas
                .Include(c => c.TipoCuota)
                .FirstOrDefaultAsync(c => c.Id_Deuda == idDeuda);
        }

        public async Task<List<CuotaComprobanteDto>> ObtenerParaComprobanteAsync(IReadOnlyCollection<int> idsCuota)
        {
            // La cuota apunta a la entidad; de ahí salen el socio (Id, plan) y sus datos
            // personales. La ciudad va con LEFT JOIN para no perder el comprobante si falta.
            // El nicho del socio (uno solo por regla de negocio) se trae como "sector-número".
            return await (
                from c in _context.Cuotas
                join tc in _context.TiposCuotas on c.Id_TipoCuota equals tc.Id_TipoCuota
                join s in _context.Socios on c.Id_Entidad equals s.Id_Entidad
                join e in _context.Entidades on c.Id_Entidad equals e.Id_Entidad
                join ci in _context.Ciudades on e.Id_Ciudad equals ci.Id_Ciudad into ciudades
                from ci in ciudades.DefaultIfEmpty()
                where idsCuota.Contains(c.Id_Deuda)
                select new CuotaComprobanteDto
                {
                    IdCuota = c.Id_Deuda,
                    IdSocio = s.Id_Socio,
                    Apellido = e.Apellido,
                    Nombre = e.Nombre,
                    Calle = e.Calle,
                    Altura = e.Altura,
                    Ciudad = ci != null ? ci.Nombre : null,
                    Plan = s.Plan,
                    Concepto = tc.Concepto,
                    Mes = c.Mes_Periodo,
                    Anio = c.Anio_Periodo,
                    Monto = c.Monto,
                    FechaVencimiento = c.FechaVencimiento,
                    Estado = c.Estado,
                    Nicho = _context.Nichos
                        .Where(n => n.Id_Entidad == c.Id_Entidad)
                        .Select(n => n.Sector + "-" + n.NroNicho)
                        .FirstOrDefault()
                }
            ).AsNoTracking().ToListAsync();
        }

        public async Task<List<Cuota>> ObtenerLoteNichoAsync(int idEntidad, DateTime fechaHoraGeneracion)
        {
            // Trackeado: el caso de uso pasa todas estas cuotas a ANULADA en una transacción.
            return await (
                from c in _context.Cuotas
                join tc in _context.TiposCuotas on c.Id_TipoCuota equals tc.Id_TipoCuota
                where c.Id_Entidad == idEntidad
                    && tc.Concepto == "NICHO"
                    && c.FechaHoraGeneracion == fechaHoraGeneracion
                select c
            ).ToListAsync();
        }
    }
}
