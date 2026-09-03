using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly SociosDbContext _context;
        public EmpleadoRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<List<EntidadDto>> ListarEmpleadosAsync(EntidadFiltroBasicoDto filtro)
        {
            var query =
                from e in _context.Entidades
                join c in _context.Ciudades on e.Id_Ciudad equals c.Id_Ciudad
                join et in _context.EntidadTipos on e.Id_Entidad equals et.Id_Entidad
                where et.Id_Tipo == 4 // solo empleados
                select new { e, c };

            if (!string.IsNullOrEmpty(filtro.Busqueda))
            {
                var patron = $"%{filtro.Busqueda.Trim()}%";
                query = query.Where(x =>
                    EF.Functions.ILike(x.e.Dni, patron) ||
                    EF.Functions.ILike(x.e.Nombre, patron) ||
                    EF.Functions.ILike(x.e.Apellido, patron));
            }

            return await query.Select(x => new EntidadDto
            {
                Id_Entidad = x.e.Id_Entidad,
                CuitCuil = x.e.CuitCuil,
                Nombre = x.e.Nombre,
                Apellido = x.e.Apellido,
                RazonSocial = x.e.RazonSocial,
                Sexo = x.e.Sexo,
                Nacimiento = x.e.Nacimiento,
                Ciudad = new Ciudad
                {
                    Id_Ciudad = x.c.Id_Ciudad,
                    Nombre = x.c.Nombre
                },
                Calle = x.e.Calle,
                Altura = x.e.Altura,
                Observacion = x.e.Observacion
            }).ToListAsync();
        }

        public async Task<EntidadDto?> DarDeBajaEmpleadoAsync(int idEntidadTipo)
        {
            var entidadTipo = await _context.EntidadTipos
                .Include(et => et.Entidad)
                .FirstOrDefaultAsync(et => et.Id_EntidadTipo == idEntidadTipo && et.Id_Tipo == 4); // solo empleados

            if (entidadTipo == null)
                return null; // no existe o no es empleado

            // Si ya estaba inactivo, podés decidir devolver null o un DTO con estado
            if (entidadTipo.Estado == "INACTIVO")
                return null;

            entidadTipo.Estado = "INACTIVO";
            await _context.SaveChangesAsync();

            return new EntidadDto
            {
                Id_Entidad = entidadTipo.Entidad.Id_Entidad,
                Nombre = entidadTipo.Entidad.Nombre,
                Apellido = entidadTipo.Entidad.Apellido
            };
        }
    }
}
