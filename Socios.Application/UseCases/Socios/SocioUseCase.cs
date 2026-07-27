using Socios.Application.DTOs;
using Socios.Application.Exceptions;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Application.UseCases.Socios
{
    /// <summary>
    /// Reúne los casos de uso del socio. Cada método coordina QUÉ hay que hacer,
    /// pero NO habla directamente con la base de datos: eso se lo delega a los
    /// repositorios y a la unidad de trabajo. Cada capa en lo suyo.
    /// </summary>
    public class SocioUseCase : ISocioUseCase
    {
        private readonly IEntidadRepository _entidades;
        private readonly ISocioRepository _socios;
        private readonly IEntidadTipoRepository _entidadesTipo;
        private readonly IContactoRepository _contactos;
        private readonly ITipoEntidadRepository _tiposEntidad;
        private readonly IUnitOfWork _unitOfWork;

        public SocioUseCase(
            IEntidadRepository entidades,
            ISocioRepository socios,
            IEntidadTipoRepository entidadesTipo,
            IContactoRepository contactos,
            ITipoEntidadRepository tiposEntidad,
            IUnitOfWork unitOfWork)
        {
            _entidades = entidades;
            _socios = socios;
            _entidadesTipo = entidadesTipo;
            _contactos = contactos;
            _tiposEntidad = tiposEntidad;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Alta de socio. Reglas:
        ///   - Si la entidad (persona) NO existe → la crea.
        ///   - Si YA existe y todavía NO es socio → la reutiliza (no la duplica).
        ///   - Si YA existe y YA es socio → corta con un error de negocio.
        /// Al final crea el Socio, su tipo "Socio" (ACTIVO) y los contactos, y confirma
        /// TODO junto en una única transacción.
        /// </summary>
        public async Task<int> CrearAsync(SocioCrearDto dto)
        {
            var dni = dto.Dni.Trim();

            // 1) ¿Ya existe una entidad (persona) con ese DNI?
            var entidad = await _entidades.ObtenerPorDniAsync(dni);

            if (entidad is null)
            {
                // No existe → la creamos a partir de los datos del formulario.
                entidad = new Entidad
                {
                    Tipo = "DNI",
                    Dni = dni,
                    Nombre = dto.Nombre.Trim(),
                    Apellido = dto.Apellido.Trim(),
                    Nacimiento = dto.FechaNacimiento,
                    Id_Ciudad = dto.IdCiudad!.Value,
                    Calle = dto.Calle.Trim(),
                    Altura = dto.Altura,
                    Observacion = dto.Observaciones?.Trim()
                };

                _entidades.Agregar(entidad);
            }

            var esSocio = await _socios.EsSocioAsync(entidad.Id_Entidad);

            if (esSocio)
            {
                // Existe y ya es socio → no se puede dar de alta dos veces.
                throw new ReglaNegocioException("Ya existe un socio registrado con ese DNI.");
            }

            // 2) Colgamos de esa entidad el socio, su tipo (ACTIVO) y los contactos.
            //    Al compartir la misma instancia de 'entidad', EF resuelve solo las claves foráneas.
            var socio = new Socio
            {
                Entidad = entidad,
                Id_OS = dto.IdObraSocial,
                Plan = dto.Plan,
                Sepelio = dto.Sepelio,
                Cobrador = dto.Cobrador,
                Numero_Afiliado = dto.NumeroAfiliado?.Trim()
            };

            _socios.Agregar(socio);

            _entidadesTipo.Agregar(new EntidadTipo
            {
                Entidad = entidad,
                Id_Tipo = 1,
                Fecha_Alta = DateTime.Today,
                Estado = "ACTIVO"
            });

            AgregarContactos(entidad, dto);

            // 3) Recién acá se confirma TODO junto (una sola transacción: todo o nada).
            await _unitOfWork.GuardarCambiosAsync();

            // El id se completa después de guardar.
            return socio.Id_Socio;
        }

        /// <summary>
        /// Visualizar un socio. Es una lectura pura: no hay reglas ni transacción, así que
        /// el trabajo (la consulta que arma el detalle) vive en el repositorio. Acá solo
        /// se delega, para mantener un único punto de entrada de las operaciones del socio.
        /// </summary>
        public async Task<SocioDetalleDto?> VisualizarAsync(int idSocio)
        {
            return await _socios.ObtenerDetalleAsync(idSocio);
        }

        /// <summary>Agrega los teléfonos y (si hay) los emails como contactos de la entidad.</summary>
        private void AgregarContactos(Entidad entidad, SocioCrearDto dto)
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
