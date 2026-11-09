using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class SocioEstadoCuentaRepository : ISocioEstadoCuentaRepository
    {
        // Tipo de entidad "Socio" en entidad_tipos (mismo valor que usa SocioRepository).
        private const int IdTipoSocio = 1;
        private const string EstadoAnulada = "ANULADA";
        private static readonly string[] EstadosAdeudados = { "PENDIENTE", "VENCIDA" };

        private readonly SociosDbContext _context;

        public SocioEstadoCuentaRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<List<SocioSeleccionDto>> ListarSociosAsync(string? busqueda)
        {
            var query = ConsultaSocios();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var filtro = busqueda.Trim().ToLower();
                query = query.Where(s =>
                    (s.Nombre != null && s.Nombre.ToLower().Contains(filtro)) ||
                    (s.Apellido != null && s.Apellido.ToLower().Contains(filtro)) ||
                    (s.Dni != null && s.Dni.Contains(filtro)));
            }

            return await query
                .OrderBy(s => s.Apellido)
                .ThenBy(s => s.Nombre)
                .ToListAsync();
        }

        public async Task<SocioSeleccionDto?> ObtenerSocioAsync(int idEntidad)
        {
            return await ConsultaSocios().FirstOrDefaultAsync(s => s.IdEntidad == idEntidad);
        }

        public async Task<List<SocioCuotaCuentaDto>> ObtenerCuotasAsync(int idEntidad, DateTime fechaDesde, DateTime fechaHasta)
        {
            var desde = fechaDesde.Date;
            var hastaExclusivo = fechaHasta.Date.AddDays(1); // incluye todo el día "hasta"

            return await ConsultaCuotas(idEntidad)
                .Where(c => c.FechaVencimiento >= desde && c.FechaVencimiento < hastaExclusivo)
                .ToListAsync();
        }

        public async Task<List<SocioCuotaCuentaDto>> ObtenerCuotasAdeudadasAsync(int idEntidad)
        {
            return await ConsultaCuotas(idEntidad)
                .Where(c => EstadosAdeudados.Contains(c.Estado))
                .ToListAsync();
        }

        public async Task<List<DeudaCuotaDto>> ObtenerDeudasAsync(int? idEntidad, DateTime? fechaDesde, DateTime? fechaHasta,
            string estadoCuota, DateTime hoy)
        {
            var query =
                from c in _context.Cuotas.AsNoTracking()
                join tc in _context.TiposCuotas on c.Id_TipoCuota equals tc.Id_TipoCuota
                join s in _context.Socios on c.Id_Entidad equals s.Id_Entidad
                join e in _context.Entidades on c.Id_Entidad equals e.Id_Entidad
                where EstadosAdeudados.Contains(c.Estado)
                select new { c, tc, s, e };

            // Estado con la regla de vencimiento del cobro (MontoCuotaACobrar.EstaVencida), igual que
            // el filtro de CuotaRepository: vencida = VENCIDA guardada, o PENDIENTE con el vencimiento
            // anterior a hoy (la que vence hoy todavía no). Si cambia la regla, cambiar los tres lugares.
            var hoyFecha = hoy.Date;
            if (estadoCuota == DeudasFiltroDto.Vencidas)
                query = query.Where(x => x.c.Estado == "VENCIDA"
                    || (x.c.Estado == "PENDIENTE" && x.c.FechaVencimiento < hoyFecha));
            else if (estadoCuota == DeudasFiltroDto.Pendientes)
                query = query.Where(x => x.c.Estado == "PENDIENTE" && x.c.FechaVencimiento >= hoyFecha);

            if (idEntidad.HasValue)
                query = query.Where(x => x.c.Id_Entidad == idEntidad.Value);

            // Filtro por fecha de vencimiento (ambas inclusive; "hasta" toma el día completo).
            if (fechaDesde.HasValue)
            {
                var desde = fechaDesde.Value.Date;
                query = query.Where(x => x.c.FechaVencimiento >= desde);
            }
            if (fechaHasta.HasValue)
            {
                var hastaExclusivo = fechaHasta.Value.Date.AddDays(1);
                query = query.Where(x => x.c.FechaVencimiento < hastaExclusivo);
            }

            return await query
                .OrderBy(x => x.e.Apellido)
                .ThenBy(x => x.e.Nombre)
                .ThenBy(x => x.e.Id_Entidad)
                .ThenBy(x => x.c.FechaVencimiento)
                .ThenBy(x => x.tc.Concepto)
                .Select(x => new DeudaCuotaDto
                {
                    IdEntidad = x.e.Id_Entidad,
                    IdSocio = x.s.Id_Socio,
                    Apellido = x.e.Apellido,
                    Nombre = x.e.Nombre,
                    Dni = x.e.Dni,
                    IdCuota = x.c.Id_Deuda,
                    Mes = x.c.Mes_Periodo,
                    Anio = x.c.Anio_Periodo,
                    TipoCuota = x.tc.Concepto,
                    FechaVencimiento = x.c.FechaVencimiento,
                    Estado = x.c.Estado,
                    MontoCuota = x.c.Monto,
                    ImporteVigente = x.tc.Importe
                })
                .ToListAsync();
        }

        /// <summary>
        /// Una fila por socio con el estado de su fila más reciente de tipo socio en
        /// entidad_tipos (puede tener varias si fue dado de baja y reactivado).
        /// </summary>
        private IQueryable<SocioSeleccionDto> ConsultaSocios()
        {
            return
                from s in _context.Socios.AsNoTracking()
                join e in _context.Entidades on s.Id_Entidad equals e.Id_Entidad
                select new SocioSeleccionDto
                {
                    IdEntidad = e.Id_Entidad,
                    IdSocio = s.Id_Socio,
                    Dni = e.Dni,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Estado = _context.EntidadTipos
                        .Where(et => et.Id_Entidad == e.Id_Entidad && et.Id_Tipo == IdTipoSocio)
                        .OrderByDescending(et => et.Id_EntidadTipo)
                        .Select(et => et.Estado)
                        .FirstOrDefault()
                };
        }

        /// <summary>
        /// Cuotas no anuladas de la entidad, con su tipo y, si están pagas, el movimiento de caja
        /// del pago (fecha, método, cuenta y observación). Ordenadas por período y vencimiento.
        /// </summary>
        private IQueryable<SocioCuotaCuentaDto> ConsultaCuotas(int idEntidad)
        {
            return
                from c in _context.Cuotas.AsNoTracking()
                join tc in _context.TiposCuotas on c.Id_TipoCuota equals tc.Id_TipoCuota
                join m in _context.Caja on c.Id_Movimiento equals (int?)m.Id_Movimiento into pagos
                from m in pagos.DefaultIfEmpty()
                join mp in _context.MetodoPago on m.Id_MetodoPago equals mp.Id_MetodoPago into metodos
                from mp in metodos.DefaultIfEmpty()
                join cu in _context.Cuenta on m.Id_Cuenta equals cu.Id_Cuenta into cuentas
                from cu in cuentas.DefaultIfEmpty()
                where c.Id_Entidad == idEntidad && c.Estado != EstadoAnulada
                orderby c.Anio_Periodo, c.Mes_Periodo, c.FechaVencimiento, tc.Concepto
                select new SocioCuotaCuentaDto
                {
                    IdCuota = c.Id_Deuda,
                    Mes = c.Mes_Periodo,
                    Anio = c.Anio_Periodo,
                    TipoCuota = tc.Concepto,
                    FechaVencimiento = c.FechaVencimiento,
                    Estado = c.Estado,
                    MontoCuota = c.Monto,
                    ImporteVigente = tc.Importe,
                    IdMovimiento = c.Id_Movimiento,
                    FechaHoraPago = m != null ? m.FechaHoraMov : null,
                    MetodoPago = mp != null ? mp.NombreMetodoPago : null,
                    Cuenta = cu != null ? cu.NombreCuenta : null,
                    NroCuenta = cu != null ? cu.NroCuenta : null,
                    Observacion = m != null ? m.Observacion : null
                };
        }
    }
}
