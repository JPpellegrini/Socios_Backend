using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class MovimientosCajaRepository : IMovimientosCajaRepository
    {
        private readonly SociosDbContext _context;

        public MovimientosCajaRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<List<ActividadCajaDto>> ListarActividadesAsync()
        {
            return await _context.DetalleMovimiento.AsNoTracking()
                .OrderBy(d => d.NombreDetalleMovimiento)
                .Select(d => new ActividadCajaDto { IdActividad = d.Id_Detmov, Nombre = d.NombreDetalleMovimiento })
                .ToListAsync();
        }

        public async Task<string?> ObtenerNombreActividadAsync(int idActividad)
        {
            return await _context.DetalleMovimiento.AsNoTracking()
                .Where(d => d.Id_Detmov == idActividad)
                .Select(d => d.NombreDetalleMovimiento)
                .FirstOrDefaultAsync();
        }

        public async Task<List<TotalCajaAgrupadoDto>> ObtenerTotalesPorActividadAsync(DateTime fechaDesde, DateTime fechaHasta)
        {
            var desde = fechaDesde.Date;
            var hastaExclusivo = fechaHasta.Date.AddDays(1); // incluye todo el día "hasta"

            // El tipo se guarda como texto libre ("Ingreso" / "INGRESO"): se normaliza al agrupar.
            return await (
                from m in _context.Caja.AsNoTracking()
                join d in _context.DetalleMovimiento on m.Id_Detmov equals d.Id_Detmov
                where m.FechaHoraMov >= desde && m.FechaHoraMov < hastaExclusivo
                group m by new { d.Id_Detmov, d.NombreDetalleMovimiento, Tipo = m.Tipo_Movimiento.Trim().ToUpper() } into g
                select new TotalCajaAgrupadoDto
                {
                    IdActividad = g.Key.Id_Detmov,
                    Actividad = g.Key.NombreDetalleMovimiento,
                    TipoMovimiento = g.Key.Tipo,
                    Cantidad = g.Count(),
                    Total = g.Sum(x => x.Monto)
                }
            ).ToListAsync();
        }

        public async Task<List<MovimientoCajaDto>> ObtenerMovimientosAsync(
            DateTime fechaDesde, DateTime fechaHasta, string? tipoMovimiento, int? idActividad)
        {
            var desde = fechaDesde.Date;
            var hastaExclusivo = fechaHasta.Date.AddDays(1); // incluye todo el día "hasta"

            var query =
                from m in _context.Caja.AsNoTracking()
                join e in _context.Entidades on m.Id_Entidad equals e.Id_Entidad
                join d in _context.DetalleMovimiento on m.Id_Detmov equals d.Id_Detmov
                join mp in _context.MetodoPago on m.Id_MetodoPago equals mp.Id_MetodoPago
                where m.FechaHoraMov >= desde && m.FechaHoraMov < hastaExclusivo
                select new { m, e, d, mp };

            // El tipo se guarda como texto libre (hay datos con "Ingreso" y con "INGRESO"):
            // se compara sin distinguir mayúsculas.
            if (tipoMovimiento is not null)
                query = query.Where(x => x.m.Tipo_Movimiento.Trim().ToUpper() == tipoMovimiento);

            if (idActividad.HasValue)
                query = query.Where(x => x.m.Id_Detmov == idActividad.Value);

            return await query
                .OrderBy(x => x.m.FechaHoraMov)
                .ThenBy(x => x.m.Id_Movimiento)
                .Select(x => new MovimientoCajaDto
                {
                    IdMovimiento = x.m.Id_Movimiento,
                    FechaHoraMovimiento = x.m.FechaHoraMov,
                    TipoMovimiento = x.m.Tipo_Movimiento.Trim().ToUpper(),
                    IdEntidad = x.e.Id_Entidad,
                    Nombre = x.e.Nombre,
                    Apellido = x.e.Apellido,
                    RazonSocial = x.e.RazonSocial,
                    Monto = x.m.Monto,
                    MetodoPago = x.mp.NombreMetodoPago,
                    IdActividad = x.d.Id_Detmov,
                    Actividad = x.d.NombreDetalleMovimiento,
                    Observacion = x.m.Observacion
                })
                .ToListAsync();
        }
    }
}
