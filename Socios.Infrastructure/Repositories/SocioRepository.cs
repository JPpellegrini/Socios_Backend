using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class SocioRepository : ISocioRepository
    {
        private readonly SociosDbContext _context;

        public SocioRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<List<SocioListadoDto>> BuscarAsync(SocioFiltroDto filtro)
        {
            // El Estado del socio vive en EntidadTipo (fila cuyo tipo es "Socio"),
            // mientras que Nombre/Apellido/Dni viven en Entidad.
            // Se asume una fila EntidadTipo de tipo "Socio" por entidad; un socio sin
            // esa fila no se lista (join interno, coherente con el modelo de datos).
            var query =
                from s in _context.Socios
                join e in _context.Entidades on s.Id_Entidad equals e.Id_Entidad
                join et in _context.EntidadTipos on e.Id_Entidad equals et.Id_Entidad
                join t in _context.TiposEntidad on et.Id_Tipo equals t.Id_Tipo
                where t.NombreTipoEntidad == "Socio"
                select new { s, e, et };

            var busqueda = filtro.Busqueda?.Trim();
            if (!string.IsNullOrEmpty(busqueda))
            {
                var patron = $"%{busqueda}%";
                query = query.Where(x =>
                    (x.e.Nombre != null && EF.Functions.ILike(x.e.Nombre, patron)) ||
                    (x.e.Apellido != null && EF.Functions.ILike(x.e.Apellido, patron)));
            }

            var dni = filtro.Dni?.Trim();
            if (!string.IsNullOrEmpty(dni))
            {
                var patron = $"%{dni}%";
                query = query.Where(x => x.e.Dni != null && EF.Functions.ILike(x.e.Dni, patron));
            }

            if (!filtro.IncluirInactivos)
            {
                query = query.Where(x => x.et.Estado == "ACTIVO");
            }

            return await query.Select(x => new SocioListadoDto
            {
                IdSocio = x.s.Id_Socio,
                Nombre = x.e.Nombre,
                Apellido = x.e.Apellido,
                Dni = x.e.Dni,
                Estado = x.et.Estado
            }).ToListAsync();
        }

        public async Task<List<SocioAsignableNichoDto>> BuscarAsignablesNichoAsync(string? busqueda)
        {
            // Regla de negocio: solo se puede asignar un nicho a socios ACTIVOS y con sepelio.
            var query =
                from s in _context.Socios
                join e in _context.Entidades on s.Id_Entidad equals e.Id_Entidad
                join et in _context.EntidadTipos on e.Id_Entidad equals et.Id_Entidad
                join t in _context.TiposEntidad on et.Id_Tipo equals t.Id_Tipo
                where t.NombreTipoEntidad == "SOCIO"
                    && et.Estado == "ACTIVO"
                    && s.Sepelio == "SI"
                select new { s, e };

            // Campo único: matchea por Nombre, Apellido o DNI (ILIKE = case-insensitive, con Trim).
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = $"%{busqueda.Trim()}%";
                query = query.Where(x =>
                    (x.e.Nombre != null && EF.Functions.ILike(x.e.Nombre, patron)) ||
                    (x.e.Apellido != null && EF.Functions.ILike(x.e.Apellido, patron)) ||
                    (x.e.Dni != null && EF.Functions.ILike(x.e.Dni, patron)));
            }

            return await query.Select(x => new SocioAsignableNichoDto
            {
                IdSocio = x.s.Id_Socio,
                IdEntidad = x.e.Id_Entidad,
                Nombre = x.e.Nombre,
                Apellido = x.e.Apellido,
                Dni = x.e.Dni
            }).ToListAsync();
        }

        public async Task<bool> EsSocioAsync(int idEntidad)
        {
            // Es socio si ya existe una fila en la tabla socios para esa entidad.
            return await _context.Socios.AnyAsync(s => s.Id_Entidad == idEntidad);
        }

        public async Task<bool> TieneSepelioAsync(int idEntidad)
        {
            return await _context.Socios.AnyAsync(s => s.Id_Entidad == idEntidad && s.Sepelio == "SI");
        }

        public async Task<List<SocioCuotaDto>> ObtenerActivosParaCuotasAsync()
        {
            // El estado ACTIVO vive en EntidadTipo (fila tipo Socio, Id_Tipo = 1).
            return await (
                from s in _context.Socios
                join e in _context.Entidades on s.Id_Entidad equals e.Id_Entidad
                join et in _context.EntidadTipos on e.Id_Entidad equals et.Id_Entidad
                where et.Id_Tipo == 1 && et.Estado == "ACTIVO"
                select new SocioCuotaDto
                {
                    IdSocio = s.Id_Socio,
                    IdEntidad = e.Id_Entidad,
                    Plan = s.Plan,
                    Sepelio = s.Sepelio,
                    Nacimiento = e.Nacimiento
                }
            ).AsNoTracking().ToListAsync();
        }

        public void Agregar(Socio socio)
        {
            // Solo marca el socio para insertar. El guardado real lo dispara la unidad de trabajo.
            _context.Socios.Add(socio);
        }

        public async Task<int?> ObtenerIdEntidadAsync(int idSocio)
        {
            return await _context.Socios
                .Where(s => s.Id_Socio == idSocio)
                .Select(s => (int?)s.Id_Entidad)
                .FirstOrDefaultAsync();
        }

        public async Task<Socio?> ObtenerConEntidadAsync(int idSocio)
        {
            // Se traen socio + entidad trackeados (sin AsNoTracking) para que el caso de uso
            // pueda modificar ambos y la unidad de trabajo confirme los cambios.
            return await _context.Socios
                .Include(s => s.Entidad)
                .FirstOrDefaultAsync(s => s.Id_Socio == idSocio);
        }

        public async Task<SocioDetalleDto?> ObtenerDetalleAsync(int idSocio)
        {
            return await (
                from s in _context.Socios
                join e in _context.Entidades on s.Id_Entidad equals e.Id_Entidad
                join c in _context.Ciudades on e.Id_Ciudad equals c.Id_Ciudad
                join et in _context.EntidadTipos on e.Id_Entidad equals et.Id_Entidad
                join os in _context.ObraSocial on s.Id_OS equals os.Id_ObraSocial into obras
                from os in obras.DefaultIfEmpty()
                join eb in _context.EntidadBajas on et.Id_EntidadTipo equals eb.Id_EntidadTipo into bajas
                from eb in bajas.DefaultIfEmpty()
                where s.Id_Socio == idSocio && et.Id_Tipo == 1
                select new SocioDetalleDto
                {
                    IdSocio = s.Id_Socio,
                    TipoDocumento = e.Tipo,
                    Dni = e.Dni,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    FechaNacimiento = e.Nacimiento,
                    Sexo = e.Sexo,
                    IdCiudad = e.Id_Ciudad,
                    Ciudad = c.Nombre,
                    Calle = e.Calle,
                    Altura = e.Altura,
                    Observaciones = e.Observacion,
                    IdObraSocial = s.Id_OS,
                    ObraSocial = os != null ? os.NombreObraSocial : null,
                    NumeroAfiliado = s.Numero_Afiliado,
                    Plan = s.Plan,
                    Sepelio = s.Sepelio,
                    Cobrador = s.Cobrador,
                    Estado = et.Estado,
                    FechaAlta = et.Fecha_Alta,
                    FechaBaja = eb != null ? eb.Fecha_Baja : null,
                    Telefonos = _context.Contactos
                        .Where(x => x.Id_Entidad == e.Id_Entidad && x.Tipo == "TELEFONO")
                        .Select(x => x.ContactoEntidad)
                        .ToList(),
                    Emails = _context.Contactos
                        .Where(x => x.Id_Entidad == e.Id_Entidad && x.Tipo == "MAIL")
                        .Select(x => x.ContactoEntidad)
                        .ToList()
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SocioInformeDto>> GenerarInformeAsync(SocioInformeFiltroDto filtro)
        {
            // No se une con EntidadBajas: el informe no muestra la fecha de baja y un socio
            // dado de baja más de una vez (con reactivación en el medio) aparecería duplicado.
            var query =
                from s in _context.Socios
                join e in _context.Entidades on s.Id_Entidad equals e.Id_Entidad
                join c in _context.Ciudades on e.Id_Ciudad equals c.Id_Ciudad
                join et in _context.EntidadTipos on e.Id_Entidad equals et.Id_Entidad
                join os in _context.ObraSocial on s.Id_OS equals os.Id_ObraSocial into obras
                from os in obras.DefaultIfEmpty()
                where et.Id_Tipo == 1 // Tipo = Socio
                select new
                {
                    s, e, c, et, os,
                    // Adherente = todo socio sin sepelio (incluye Sepelio nulo, no solo "NO").
                    TieneSepelio = s.Sepelio != null && s.Sepelio.ToUpper() == "SI",
                    TieneNicho = _context.Nichos.Any(n => n.Id_Entidad == e.Id_Entidad)
                };

            // Filtro por estado (el controlador ya validó y normalizó el valor).
            if (filtro.Estado != "TODOS")
                query = query.Where(x => x.et.Estado == filtro.Estado);

            // Filtro por categoría: se combinan con OR (se listan los socios que cumplan
            // alguna de las marcadas). "Sepelio" excluye a quienes tienen nicho, para que
            // "solo sepelio" y "solo nicho" no se superpongan.
            var adherentes = filtro.IncluirAdherentes;
            var sepelio = filtro.IncluirSepelio;
            var nicho = filtro.IncluirNicho;
            if (!(adherentes && sepelio && nicho))
            {
                query = query.Where(x =>
                    (adherentes && !x.TieneSepelio) ||
                    (sepelio && x.TieneSepelio && !x.TieneNicho) ||
                    (nicho && x.TieneNicho));
            }

            var hoy = DateTime.Today;

            return await query
                .OrderBy(x => x.e.Apellido)
                .ThenBy(x => x.e.Nombre)
                .Select(x => new SocioInformeDto
                {
                    Nombre = x.e.Nombre,
                    Apellido = x.e.Apellido,
                    Calle = x.e.Calle,
                    Altura = x.e.Altura.ToString(),
                    Ciudad = x.c.Nombre,
                    Telefono = _context.Contactos
                                    .Where(con => con.Id_Entidad == x.e.Id_Entidad && con.Tipo == "TELEFONO")
                                    .Select(con => con.ContactoEntidad)
                                    .FirstOrDefault(),
                    Mail = _context.Contactos
                                    .Where(con => con.Id_Entidad == x.e.Id_Entidad && con.Tipo == "MAIL")
                                    .Select(con => con.ContactoEntidad)
                                    .FirstOrDefault(),
                    ObraSocial = x.os != null ? x.os.NombreObraSocial : null,
                    // Edad exacta: se resta un año si todavía no cumplió en el año actual.
                    Edad = x.e.Nacimiento.HasValue
                                    ? hoy.Year - x.e.Nacimiento.Value.Year
                                        - (x.e.Nacimiento.Value.Month > hoy.Month
                                            || (x.e.Nacimiento.Value.Month == hoy.Month && x.e.Nacimiento.Value.Day > hoy.Day)
                                            ? 1 : 0)
                                    : null,
                    Sepelio = x.TieneSepelio,
                    Nicho = x.TieneNicho,
                    Estado = x.et.Estado,
                    TipoSocio = x.TieneSepelio ? "Sepelio" : "Adherente"
                }).AsNoTracking().ToListAsync();
        }

        public async Task<List<SocioMovimientoDto>> ObtenerMovimientosAsync(DateTime desde, DateTime hasta, bool incluirAltas, bool incluirBajas)
        {
            var hoy = DateTime.Today;
            desde = desde.Date;
            hasta = hasta.Date;

            // Al pedir solo altas se listan las altas vigentes: se excluye a quien ya fue dado de
            // baja (tenga registro de baja, en cualquier fecha, o figure como INACTIVO).
            var soloVigentes = incluirAltas && !incluirBajas;

            // Una sola consulta: cada socio con su baja (si tiene). Trae a los que se dieron de
            // alta en el período y/o a los que se dieron de baja en el período; después, en
            // memoria, se deja una sola fila por socio marcando qué movimientos tuvo.
            var socios = await (
                from s in _context.Socios
                join e in _context.Entidades on s.Id_Entidad equals e.Id_Entidad
                join c in _context.Ciudades on e.Id_Ciudad equals c.Id_Ciudad
                join et in _context.EntidadTipos on e.Id_Entidad equals et.Id_Entidad
                join os in _context.ObraSocial on s.Id_OS equals os.Id_ObraSocial into obras
                from os in obras.DefaultIfEmpty()
                join eb in _context.EntidadBajas on et.Id_EntidadTipo equals eb.Id_EntidadTipo into bajas
                from eb in bajas.DefaultIfEmpty()
                where et.Id_Tipo == 1 // Tipo = Socio
                    && ((incluirAltas && et.Fecha_Alta >= desde && et.Fecha_Alta <= hasta
                            && (!soloVigentes || (et.Estado == "ACTIVO" && eb == null)))
                        || (incluirBajas && eb != null && eb.Fecha_Baja >= desde && eb.Fecha_Baja <= hasta))
                select new SocioMovimientoDto
                {
                    IdSocio = s.Id_Socio,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Calle = e.Calle,
                    Altura = e.Altura.ToString(),
                    Ciudad = c.Nombre,
                    Telefono = _context.Contactos
                                    .Where(con => con.Id_Entidad == e.Id_Entidad && con.Tipo == "TELEFONO")
                                    .Select(con => con.ContactoEntidad)
                                    .FirstOrDefault(),
                    ObraSocial = os != null ? os.NombreObraSocial : null,
                    // Edad exacta (a hoy), igual que en el informe de socios.
                    Edad = e.Nacimiento.HasValue
                                    ? hoy.Year - e.Nacimiento.Value.Year
                                        - (e.Nacimiento.Value.Month > hoy.Month
                                            || (e.Nacimiento.Value.Month == hoy.Month && e.Nacimiento.Value.Day > hoy.Day)
                                            ? 1 : 0)
                                    : null,
                    Sepelio = s.Sepelio != null && s.Sepelio.ToUpper() == "SI",
                    Nicho = _context.Nichos.Any(n => n.Id_Entidad == e.Id_Entidad),
                    FechaAlta = et.Fecha_Alta,
                    FechaBaja = eb != null ? eb.Fecha_Baja : null,
                    MotivoBaja = eb != null ? eb.Motivo : null
                }).AsNoTracking().ToListAsync();

            bool EnPeriodo(DateTime? fecha) => fecha >= desde && fecha <= hasta;

            var resultado = new List<SocioMovimientoDto>();

            // Una fila por socio. Si tuviera más de una baja (la consulta trae una fila por
            // baja), se queda con la más reciente dentro del período o, si no hay, la más reciente.
            foreach (var filas in socios.GroupBy(s => s.IdSocio))
            {
                var socio = filas
                    .OrderByDescending(f => incluirBajas && EnPeriodo(f.FechaBaja))
                    .ThenByDescending(f => f.FechaBaja)
                    .First();

                socio.AltaEnPeriodo = incluirAltas && EnPeriodo(socio.FechaAlta);
                socio.BajaEnPeriodo = incluirBajas && EnPeriodo(socio.FechaBaja);

                // Si se dio de baja en el período, la fila muestra la baja (con su motivo);
                // si no, el alta. Los totales del resumen igual cuentan ambos movimientos.
                socio.Movimiento = socio.BajaEnPeriodo ? "BAJA" : "ALTA";
                socio.Fecha = socio.BajaEnPeriodo ? socio.FechaBaja!.Value : socio.FechaAlta;

                resultado.Add(socio);
            }

            return resultado
                .OrderBy(m => m.Fecha)
                .ThenBy(m => m.Apellido)
                .ThenBy(m => m.Nombre)
                .ToList();
        }
    }
}
