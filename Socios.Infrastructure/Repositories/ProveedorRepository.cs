using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Infrastructure.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly SociosDbContext _context;

        public ProveedorRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProveedorListadoDto>> BuscarAsync(ProveedorFiltroDto filtro)
        {
            // El Estado del proveedor vive en EntidadTipo (fila cuyo tipo es "Proveedor", Id_Tipo = 2),
            // mientras que los datos personales/empresa viven en Entidad y el servicio en Prestacion.
            // Se asume una fila EntidadTipo de tipo "Proveedor" por entidad (join interno).
            var query =
                from p in _context.Proveedores
                join e in _context.Entidades on p.Id_Entidad equals e.Id_Entidad
                join c in _context.Ciudades on e.Id_Ciudad equals c.Id_Ciudad
                join pr in _context.Prestacion on p.Id_Prestacion equals pr.Id_Prestacion
                join et in _context.EntidadTipos on e.Id_Entidad equals et.Id_Entidad
                join t in _context.TiposEntidad on et.Id_Tipo equals t.Id_Tipo
                where t.NombreTipoEntidad == "Proveedor"
                select new { p, e, c, pr, et };

            var busqueda = filtro.Busqueda?.Trim();
            if (!string.IsNullOrEmpty(busqueda))
            {
                var patron = $"%{busqueda}%";
                query = query.Where(x =>
                    (x.e.Nombre != null && EF.Functions.ILike(x.e.Nombre, patron)) ||
                    (x.e.Apellido != null && EF.Functions.ILike(x.e.Apellido, patron)) ||
                    (x.e.RazonSocial != null && EF.Functions.ILike(x.e.RazonSocial, patron)));
            }

            var cuitCuil = filtro.CuitCuil?.Trim();
            if (!string.IsNullOrEmpty(cuitCuil))
            {
                var patron = $"%{cuitCuil}%";
                query = query.Where(x => x.e.CuitCuil != null && EF.Functions.ILike(x.e.CuitCuil, patron));
            }

            if (!filtro.IncluirInactivos)
            {
                query = query.Where(x => x.et.Estado == "ACTIVO");
            }

            return await query.Select(x => new ProveedorListadoDto
            {
                IdProveedor = x.p.Id_Proveedor,
                IdEntidad = x.e.Id_Entidad,
                TipoDocumento = x.e.Tipo,
                Dni = x.e.Dni,
                CuitCuil = x.e.CuitCuil,
                Nombre = x.e.Nombre,
                Apellido = x.e.Apellido,
                RazonSocial = x.e.RazonSocial,
                IdCiudad = x.e.Id_Ciudad,
                Ciudad = x.c.Nombre,
                Calle = x.e.Calle,
                Altura = x.e.Altura,
                Observacion = x.e.Observacion,
                IdPrestacion = x.pr.Id_Prestacion,
                ServicioPrestado = x.pr.NombrePrestacion,
                Estado = x.et.Estado
            }).ToListAsync();
        }

        public async Task<ProveedorDetalleDto?> ObtenerDetalleAsync(int idEntidad)
        {
            return await (
                from p in _context.Proveedores
                join e in _context.Entidades on p.Id_Entidad equals e.Id_Entidad
                join c in _context.Ciudades on e.Id_Ciudad equals c.Id_Ciudad
                join pr in _context.Prestacion on p.Id_Prestacion equals pr.Id_Prestacion
                join et in _context.EntidadTipos on e.Id_Entidad equals et.Id_Entidad
                join eb in _context.EntidadBajas on et.Id_EntidadTipo equals eb.Id_EntidadTipo into bajas
                from eb in bajas.DefaultIfEmpty()
                where p.Id_Entidad == idEntidad && et.Id_Tipo == 2
                select new ProveedorDetalleDto
                {
                    IdProveedor = p.Id_Proveedor,
                    IdEntidad = e.Id_Entidad,
                    TipoDocumento = e.Tipo,
                    Dni = e.Dni,
                    CuitCuil = e.CuitCuil,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    RazonSocial = e.RazonSocial,
                    IdCiudad = e.Id_Ciudad,
                    Ciudad = c.Nombre,
                    Calle = e.Calle,
                    Altura = e.Altura,
                    Observaciones = e.Observacion,
                    IdPrestacion = pr.Id_Prestacion,
                    ServicioPrestado = pr.NombrePrestacion,
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

        public async Task<bool> EsProveedorAsync(int idEntidad)
        {
            // Es proveedor si ya existe una fila en la tabla proveedores para esa entidad.
            return await _context.Proveedores.AnyAsync(p => p.Id_Entidad == idEntidad);
        }

        public void Agregar(Proveedor proveedor)
        {
            // Solo marca el proveedor para insertar. El guardado real lo dispara la unidad de trabajo.
            _context.Proveedores.Add(proveedor);
        }

        public async Task<Proveedor?> ObtenerConEntidadPorEntidadAsync(int idEntidad)
        {
            // Se traen proveedor + entidad trackeados (sin AsNoTracking) para que el caso de uso
            // pueda modificar ambos y la unidad de trabajo confirme los cambios.
            return await _context.Proveedores
                .Include(p => p.Entidad)
                .FirstOrDefaultAsync(p => p.Id_Entidad == idEntidad);
        }
    }
}
