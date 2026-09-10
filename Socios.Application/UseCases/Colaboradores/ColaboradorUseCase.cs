using Socios.Application.DTOs;
using Socios.Application.Exceptions;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Application.UseCases.Colaboradores
{
    public class ColaboradorUseCase : IColaboradorUseCase
    {
        private readonly IColaboradorRepository _colaboradores;
        private readonly IContactoRepository _contactos;
        private readonly IUnitOfWork _unitOfWork;

        public ColaboradorUseCase(
            IColaboradorRepository colaboradores,
            IContactoRepository contactos,
            IUnitOfWork unitOfWork)
        {
            _colaboradores = colaboradores;
            _contactos = contactos;
            _unitOfWork = unitOfWork;
        }
       
        public async Task<List<ColaboradorDto>> BuscarAsync(string? busqueda)
        {
            var colaboradores = await _colaboradores.BuscarAsync(busqueda);

            return colaboradores.Select(e => new ColaboradorDto
            {
                Id_Entidad = e.Id_Entidad,
                Dni = e.Dni,
                CuitCuil = e.CuitCuil,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Sexo = e.Sexo,
                Nacimiento = e.Nacimiento,
                Calle = e.Calle,
                Altura = e.Altura,
                Observacion = e.Observacion,
                Ciudad = e.Ciudad,
                Telefonos = e.Contactos
                    .Where(c => c.Tipo == "TELEFONO")
                    .Select(c => c.ContactoEntidad)
                    .ToList(),
                Emails = e.Contactos
                    .Where(c => c.Tipo == "MAIL")
                    .Select(c => c.ContactoEntidad)
                    .ToList()
            }).ToList();
        }
        /// <summary>
        /// Baja de empleado
        /// </summary>
        public async Task<bool> BajaAsync(int idEntidadTipo, string motivo)
        {
            var entidadTipo = await _colaboradores.ObtenerEntidadTipoAsync(idEntidadTipo);

            if (entidadTipo is null || entidadTipo.Estado == "INACTIVO")
                return false;

            // 1. Cambiar estado
            entidadTipo.Estado = "INACTIVO";
            _colaboradores.ActualizarEntidadTipo(entidadTipo);

            // 2. Registrar baja en tabla EntidadesBaja
            var baja = new EntidadBaja
            {
                Id_EntidadTipo = idEntidadTipo,
                Fecha_Baja = DateTime.Now,
                Motivo = motivo
            };
            await _colaboradores.RegistrarBajaAsync(baja);

            // 3. Guardar cambios
            await _unitOfWork.GuardarCambiosAsync();

            return true;
        }
    }
}