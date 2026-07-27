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

        public async Task<Socio> AddAsync(Socio socio)
        {
            _context.Add(socio);
            await _context.SaveChangesAsync();
            return socio;
        }

        public async Task DeleteAsync(int idEntidad)
        {
            var socio = await _context.Set<Socio>().FindAsync(idEntidad);
            if (socio != null)
            {
                _context.Remove(socio);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Socio>> GetAllAsync()
        {
            return await _context.Set<Socio>().Include(s => s.ObraSocial).Include(s => s.Entidad).ToListAsync();
        }

        public async Task<Socio?> GetByIdAsync(int idEntidad)
        {
            return await _context.Set<Socio>().Include(s => s.ObraSocial).Include(s => s.Entidad).FirstOrDefaultAsync(s => s.Id_Entidad == idEntidad);
        }

        public async Task UpdateAsync(Socio socio)
        {
            _context.Update(socio);
            await _context.SaveChangesAsync();
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

        public async Task<bool> EsSocioAsync(int idEntidad)
        {
            // Es socio si ya existe una fila en la tabla socios para esa entidad.
            return await _context.Socios.AnyAsync(s => s.Id_Entidad == idEntidad);
        }

        public void Agregar(Socio socio)
        {
            // Solo marca el socio para insertar. El guardado real lo dispara la unidad de trabajo.
            _context.Socios.Add(socio);
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

        public async Task DarDeBajaAsync(SocioBajaDto dto)
        {
            var socio = await (
                from s in _context.Socios
                join et in _context.EntidadTipos on s.Id_Entidad equals et.Id_Entidad
                where s.Id_Socio == dto.IdSocio && et.Id_Tipo == 1
                select new { s, et }
            ).FirstOrDefaultAsync();

            if (socio == null)
                throw new Exception("No se encontro el socio con ese Id.");

            socio.et.Estado = "INACTIVO";

            var baja = new EntidadBaja
            {
                Id_EntidadTipo = socio.et.Id_EntidadTipo,
                Fecha_Baja = DateTime.Now,
                Motivo = dto.Motivo
            };

            _context.EntidadBajas.Add(baja);
            _context.EntidadTipos.Update(socio.et);

            await _context.SaveChangesAsync();
        }


    }
}
