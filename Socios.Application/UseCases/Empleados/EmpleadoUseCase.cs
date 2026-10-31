using Socios.Application.DTOs;
using Socios.Application.Exceptions;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Application.UseCases.Empleados
{
    public class EmpleadoUseCase : IEmpleadoUseCase
    {
        private readonly IEmpleadoRepository _empleados;
        private readonly IContactoRepository _contactos;
        private readonly IUnitOfWork _unitOfWork;

        public EmpleadoUseCase(
            IEmpleadoRepository empleados,
            IContactoRepository contactos,
            IUnitOfWork unitOfWork)
        {
            _empleados = empleados;
            _contactos = contactos;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Alta de empleado
        /// </summary>
        public async Task<int> CrearAsync(EmpleadoCrearDto dto)
        {
            var dni = dto.Dni.Trim();

            // 1) ¿Ya existe la entidad?
            var entidad = await _empleados.ObtenerPorDniAsync(dni);

            if (entidad is null)
            {
                entidad = new Entidad
                {
                    Tipo = "DNI",
                    Dni = dni,
                    Nombre = dto.Nombre.Trim(),
                    Apellido = dto.Apellido.Trim(),
                    Nacimiento = dto.Nacimiento,
                    Sexo = dto.Sexo,
                    Id_Ciudad = dto.IdCiudad!.Value,
                    Calle = dto.Calle.Trim(),
                    Altura = dto.Altura,
                    Observacion = dto.Observaciones?.Trim()
                };

                _empleados.AgregarEntidad(entidad);
            }

            // 2) Verificar si ya es empleado
            if (await _empleados.EsEmpleadoAsync(entidad.Id_Entidad))
                throw new ReglaNegocioException("Ya existe un empleado registrado con ese DNI.");

            // 3) Colgar tipo "Empleado"
            _empleados.AgregarEntidadTipo(new EntidadTipo
            {
                Entidad = entidad,
                Id_Tipo = 4,
                Fecha_Alta = DateTime.Today,
                Estado = "ACTIVO"
            });

            // 4) Agregar contactos
            AgregarContactos(entidad, dto.Telefonos, dto.Emails);

            // 5) Confirmar todo junto
            await _unitOfWork.GuardarCambiosAsync();

            return entidad.Id_Entidad;
        }

        /// <summary>
        /// Visualizar empleado
        /// </summary>
        public async Task<EmpleadoDto?> VisualizarAsync(int idEntidad)
        {
            var entidad = await _empleados.ObtenerPorIdAsync(idEntidad);

            if (entidad is null)
                return null;

            return new EmpleadoDto
            {
                Id_Entidad = entidad.Id_Entidad,
                Dni = entidad.Dni,
                CuitCuil = entidad.CuitCuil,
                Nombre = entidad.Nombre,
                Apellido = entidad.Apellido,
                Nacimiento = entidad.Nacimiento,
                Calle = entidad.Calle,
                Altura = entidad.Altura,
                Observacion = entidad.Observacion,
                Ciudad = entidad.Ciudad
            };
        }
        /// <summary>
        /// Baja de empleado
        /// </summary>
        public async Task<bool> BajaAsync(int idEntidadTipo, string motivo)
        {
            var entidadTipo = await _empleados.ObtenerEntidadTipoAsync(idEntidadTipo);

            if (entidadTipo is null || entidadTipo.Estado == "INACTIVO")
                return false;

            // 1. Cambiar estado
            entidadTipo.Estado = "INACTIVO";
            _empleados.ActualizarEntidadTipo(entidadTipo);

            // 2. Registrar baja en tabla EntidadesBaja
            var baja = new EntidadBaja
            {
                Id_EntidadTipo = idEntidadTipo,
                Fecha_Baja = DateTime.Now,
                Motivo = motivo
            };
            await _empleados.RegistrarBajaAsync(baja);

            // 3. Guardar cambios
            await _unitOfWork.GuardarCambiosAsync();

            return true;
        }

        /// <summary>
        /// Modificar empleado
        /// </summary>
        public async Task ModificarAsync(EmpleadoModificarDto dto)
        {
            var entidad = await _empleados.ObtenerPorIdAsync(dto.IdEntidad)
                ?? throw new InvalidOperationException("No se encontró el empleado indicado.");

            // Datos personales
            if (!string.IsNullOrWhiteSpace(dto.Dni))
                entidad.Dni = dto.Dni.Trim();

            if (!string.IsNullOrWhiteSpace(dto.CuitCuil))
                entidad.CuitCuil = dto.CuitCuil.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                entidad.Nombre = dto.Nombre.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Apellido))
                entidad.Apellido = dto.Apellido.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Sexo))
                entidad.Sexo = dto.Sexo.Trim();

            if (dto.Nacimiento.HasValue)
                entidad.Nacimiento = dto.Nacimiento.Value;

            // Domicilio
            if (dto.IdCiudad.HasValue)
                entidad.Id_Ciudad = dto.IdCiudad.Value;

            if (!string.IsNullOrWhiteSpace(dto.Calle))
                entidad.Calle = dto.Calle.Trim();

            if (dto.Altura.HasValue)
                entidad.Altura = dto.Altura;

            if (!string.IsNullOrWhiteSpace(dto.Observaciones))
                entidad.Observacion = dto.Observaciones.Trim();

            // Contactos (reemplazo completo)
            var telefonosExistentes = entidad.Contactos
                .Where(c => c.Tipo == "TELEFONO")
                .ToList();
            foreach (var tel in telefonosExistentes)
                entidad.Contactos.Remove(tel);

            var mailsExistentes = entidad.Contactos
                .Where(c => c.Tipo == "MAIL")
                .ToList();
            foreach (var mail in mailsExistentes)
                entidad.Contactos.Remove(mail);

            _empleados.ActualizarEntidad(entidad);

            // Reemplazar contactos
            var contactosActuales = await _contactos.ObtenerPorEntidadAsync(entidad.Id_Entidad);
            foreach (var contacto in contactosActuales)
                _contactos.Eliminar(contacto);

            AgregarContactos(entidad, dto.Telefonos, dto.Emails);

            await _unitOfWork.GuardarCambiosAsync();
        }

        private void AgregarContactos(Entidad entidad, List<string> telefonos, List<string>? emails)
        {
            foreach (var telefono in telefonos)
            {
                _contactos.Agregar(new Contacto
                {
                    Entidad = entidad,
                    Tipo = "TELEFONO",
                    ContactoEntidad = telefono.Trim()
                });
            }

            if (emails is not null)
            {
                foreach (var email in emails)
                {
                    _contactos.Agregar(new Contacto
                    {
                        Entidad = entidad,
                        Tipo = "MAIL",
                        ContactoEntidad = email.Trim()
                    });
                }
            }
        }

        public async Task<List<EmpleadoDto>> BuscarAsync(string? busqueda)
        {
            var empleados = await _empleados.BuscarAsync(busqueda);

            return empleados.Select(e => new EmpleadoDto
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
    }
}