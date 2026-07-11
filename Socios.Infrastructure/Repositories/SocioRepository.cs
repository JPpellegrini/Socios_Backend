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

        public async Task<int> CrearAsync(SocioCrearDto dto)
        {
            var entidad = new Entidad
            {
                Tipo = "DNI",
                Dni = dto.Dni.Trim(),
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Nacimiento = dto.FechaNacimiento,
                Id_Ciudad = dto.IdCiudad!.Value,
                Calle = dto.Calle.Trim(),
                Altura = dto.Altura,
                Observacion = dto.Observaciones?.Trim()
            };

            var socio = new Socio
            {
                Entidad = entidad,
                Id_OS = dto.IdObraSocial,
                Plan = dto.Plan,
                Sepelio = dto.Sepelio,
                Cobrador = dto.Cobrador,
                Numero_Afiliado = dto.NumeroAfiliado?.Trim()
            };

            var entidadTipo = new EntidadTipo
            {
                Entidad = entidad,
                Id_Tipo = 1, //Id del tipo "Socio" en la tabla TiposEntidad
                Fecha_Alta = DateTime.Today,
                Estado = "ACTIVO"
            };

            _context.Socios.Add(socio);
            _context.EntidadTipos.Add(entidadTipo);

            foreach (var telefono in dto.Telefonos)
            {
                _context.Contactos.Add(new Contacto
                {
                    Entidad = entidad,
                    Tipo = "TELEFONO",
                    ContactoEntidad = telefono.Trim()
                });
            }

            if (dto.Emails != null)
            {
                foreach (var email in dto.Emails)
                {
                    _context.Contactos.Add(new Contacto
                    {
                        Entidad = entidad,
                        Tipo = "MAIL",
                        ContactoEntidad = email.Trim()
                    });
                }
            }

            await _context.SaveChangesAsync();

            return socio.Id_Socio;
        }
    }
}
