using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Application.UseCases.Entidades
{
    /// <summary>
    /// Reúne los casos de uso de la entidad (persona). Cada método coordina QUÉ hay que hacer,
    /// pero NO habla directamente con la base de datos: eso se lo delega a los repositorios y a
    /// la unidad de trabajo. Cada capa en lo suyo.
    /// </summary>
    public class EntidadUseCase : IEntidadUseCase
    {
        private readonly IEntidadRepository _entidades;
        private readonly IContactoRepository _contactos;
        private readonly IUnitOfWork _unitOfWork;

        public EntidadUseCase(
            IEntidadRepository entidades,
            IContactoRepository contactos,
            IUnitOfWork unitOfWork)
        {
            _entidades = entidades;
            _contactos = contactos;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Alta de entidad pura. Reglas:
        ///   - Si NO existe una entidad con ese DNI → la crea.
        ///   - Si YA existe → la reutiliza (no la duplica).
        /// En ambos casos agrega los contactos y confirma TODO junto en una única transacción.
        /// Devuelve el Id de la entidad.
        /// </summary>
        public async Task<int> CrearAsync(EntidadCrearDto dto)
        {
            var dni = dto.Dni.Trim();

            // ¿Ya existe una entidad (persona) con ese DNI? Si no, la creamos.
            var entidad = await _entidades.ObtenerPorDniAsync(dni);

            if (entidad is null)
            {
                entidad = new Entidad
                {
                    Tipo = "DNI",
                    Dni = dni,
                    Nombre = dto.Nombre.Trim(),
                    Apellido = dto.Apellido.Trim(),
                    Sexo = dto.Sexo?.Trim(),
                    Nacimiento = dto.FechaNacimiento,
                    Id_Ciudad = dto.IdCiudad!.Value,
                    Calle = dto.Calle.Trim(),
                    Altura = dto.Altura,
                    Observacion = dto.Observaciones?.Trim()
                };

                _entidades.Agregar(entidad);
            }

            AgregarContactos(entidad, dto);

            // Recién acá se confirma TODO junto (una sola transacción: todo o nada).
            await _unitOfWork.GuardarCambiosAsync();

            // El id se completa después de guardar.
            return entidad.Id_Entidad;
        }

        /// <summary>Agrega los teléfonos y (si hay) los emails como contactos de la entidad.</summary>
        private void AgregarContactos(Entidad entidad, EntidadCrearDto dto)
        {
            foreach (var telefono in dto.Telefonos)
            {
                _contactos.Agregar(new Contacto
                {
                    Entidad = entidad,
                    Tipo = "TELEFONO",
                    ContactoEntidad = telefono.Trim()
                });
            }

            if (dto.Emails is not null)
            {
                foreach (var email in dto.Emails)
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
    }
}
