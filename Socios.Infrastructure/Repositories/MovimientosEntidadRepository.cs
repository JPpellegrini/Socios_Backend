using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class MovimientosEntidadRepository : IMovimientosEntidadRepository
    {
        private readonly SociosDbContext _context;

        public MovimientosEntidadRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<List<EntidadMovimientosDto>> ListarEntidadesAsync(TipoEntidadMovimientos tipo)
        {
            return await ConsultaEntidades(tipo)
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ThenBy(e => e.RazonSocial)
                .ToListAsync();
        }

        public async Task<EntidadMovimientosDto?> ObtenerEntidadAsync(int idEntidad, TipoEntidadMovimientos tipo)
        {
            return await ConsultaEntidades(tipo).FirstOrDefaultAsync(e => e.IdEntidad == idEntidad);
        }

        public async Task<List<MovimientoEntidadDto>> ObtenerMovimientosAsync(
            int idEntidad, TipoEntidadMovimientos tipo, DateTime fechaDesde, DateTime fechaHasta)
        {
            var idsDetalle = tipo.IdsDetalleMovimiento.ToArray();
            var desde = fechaDesde.Date;
            var hastaExclusivo = fechaHasta.Date.AddDays(1); // incluye todo el día "hasta"

            return await (
                from m in _context.Caja.AsNoTracking()
                join d in _context.DetalleMovimiento on m.Id_Detmov equals d.Id_Detmov
                join mp in _context.MetodoPago on m.Id_MetodoPago equals mp.Id_MetodoPago
                join cu in _context.Cuenta on m.Id_Cuenta equals cu.Id_Cuenta
                where m.Id_Entidad == idEntidad
                      && idsDetalle.Contains(m.Id_Detmov)
                      && m.FechaHoraMov >= desde
                      && m.FechaHoraMov < hastaExclusivo
                orderby m.FechaHoraMov, m.Id_Movimiento
                select new MovimientoEntidadDto
                {
                    IdMovimiento = m.Id_Movimiento,
                    FechaHoraMovimiento = m.FechaHoraMov,
                    Concepto = d.NombreDetalleMovimiento,
                    TipoMovimiento = m.Tipo_Movimiento,
                    Monto = m.Monto,
                    MetodoPago = mp.NombreMetodoPago,
                    Cuenta = cu.NombreCuenta,
                    NroCuenta = cu.NroCuenta,
                    Observacion = m.Observacion
                }
            ).ToListAsync();
        }

        /// <summary>
        /// Una fila por entidad del tipo indicado, con la prestación que brinda (desde la tabla
        /// de colaboradores o de proveedores) y el estado de su fila más reciente en
        /// entidad_tipos (puede tener varias si fue dada de baja y reactivada).
        /// </summary>
        private IQueryable<EntidadMovimientosDto> ConsultaEntidades(TipoEntidadMovimientos tipo)
        {
            var idTipo = tipo.IdTipoEntidad;

            // Prestación según el tipo: cada uno la guarda en su propia tabla.
            IQueryable<EntidadPrestacion> prestaciones = tipo == TipoEntidadMovimientos.Colaborador
                ? _context.Colaboradores.Select(c => new EntidadPrestacion { IdEntidad = c.Id_Entidad, IdPrestacion = c.Id_Prestacion })
                : _context.Proveedores.Select(p => new EntidadPrestacion { IdEntidad = p.Id_Entidad, IdPrestacion = p.Id_Prestacion });

            return
                from e in _context.Entidades.AsNoTracking()
                where _context.EntidadTipos.Any(et => et.Id_Entidad == e.Id_Entidad && et.Id_Tipo == idTipo)
                select new EntidadMovimientosDto
                {
                    IdEntidad = e.Id_Entidad,
                    Dni = e.Dni,
                    CuitCuil = e.CuitCuil,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    RazonSocial = e.RazonSocial,
                    Prestacion = (
                        from pr in prestaciones
                        join p in _context.Prestacion on pr.IdPrestacion equals p.Id_Prestacion
                        where pr.IdEntidad == e.Id_Entidad
                        select p.NombrePrestacion
                    ).FirstOrDefault(),
                    Estado = _context.EntidadTipos
                        .Where(et => et.Id_Entidad == e.Id_Entidad && et.Id_Tipo == idTipo)
                        .OrderByDescending(et => et.Id_EntidadTipo)
                        .Select(et => et.Estado)
                        .FirstOrDefault()
                };
        }

        private sealed class EntidadPrestacion
        {
            public int IdEntidad { get; init; }
            public int IdPrestacion { get; init; }
        }
    }
}
