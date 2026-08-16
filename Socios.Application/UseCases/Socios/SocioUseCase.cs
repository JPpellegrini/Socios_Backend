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
        private readonly IEntidadBajaRepository _entidadesBaja;
        private readonly ICodeudorRepository _codeudores;
        private readonly IUnitOfWork _unitOfWork;

        public SocioUseCase(
            IEntidadRepository entidades,
            ISocioRepository socios,
            IEntidadTipoRepository entidadesTipo,
            IContactoRepository contactos,
            ITipoEntidadRepository tiposEntidad,
            IEntidadBajaRepository entidadesBaja,
            ICodeudorRepository codeudores,
            IUnitOfWork unitOfWork)
        {
            _entidades = entidades;
            _socios = socios;
            _entidadesTipo = entidadesTipo;
            _contactos = contactos;
            _tiposEntidad = tiposEntidad;
            _entidadesBaja = entidadesBaja;
            _codeudores = codeudores;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Alta de socio. Reglas:
        ///   - Si la entidad (persona) NO existe → la crea.
        ///   - Si YA existe y todavía NO es socio → la reutiliza (no la duplica).
        ///   - Si YA existe y YA es socio → corta con un error de negocio.
        /// Al final crea el Socio, su tipo "Socio" (ACTIVO), los contactos y la relación con
        /// sus codeudores (al menos uno), y confirma TODO junto en una única transacción.
        /// </summary>
        public async Task<int> CrearAsync(SocioCrearDto dto)
        {
            // Regla de negocio: un socio no puede darse de alta sin al menos un codeudor.
            if (dto.Codeudores is null || dto.Codeudores.Count == 0)
                throw new ReglaNegocioException("Debe asignar al menos un codeudor para dar de alta el socio.");

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

            AgregarContactos(entidad, dto.Telefonos, dto.Emails);

            // 3) Relación con los codeudores. El id con el que se da de alta es el Id_Entidad del
            //    socio (el avalado) y el Id_EntidadCodeudor de la persona que avala: hasta acá la
            //    relación no existe. Al compartir la instancia 'entidad', EF resuelve la clave del
            //    socio aunque todavía no esté guardada.
            foreach (var idEntidadCodeudor in dto.Codeudores)
            {
                // El codeudor tiene que ser una entidad que ya exista.
                if (!await _entidades.ExisteAsync(idEntidadCodeudor))
                    throw new ReglaNegocioException($"No existe una entidad con el Id {idEntidadCodeudor} para asignar como codeudor.");

                _codeudores.Agregar(new Codeudor
                {
                    Entidad = entidad,                          // el socio avalado
                    Id_EntidadCodeudor = idEntidadCodeudor      // la persona que avala (ya existe)
                });
            }

            // 4) Recién acá se confirma TODO junto (una sola transacción: todo o nada).
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

        public async Task BajaAsync(SocioBajaDto dto)
        {
            // 1) Ubicar la entidad del socio.
            var idEntidad = await _socios.ObtenerIdEntidadAsync(dto.IdSocio)?? 
                throw new RecursoNoEncontradoException("No se encontró el socio indicado.");

            // 2) Ubicar su fila de tipo "Socio" (sin números mágicos).
            var idTipoSocio = await _tiposEntidad.ObtenerIdPorNombreAsync("Socio")
                ?? throw new InvalidOperationException("No está configurado el tipo de entidad 'Socio'.");

            var entidadTipo = await _entidadesTipo.ObtenerPorEntidadYTipoAsync(idEntidad, idTipoSocio)
                ?? throw new RecursoNoEncontradoException("El socio no tiene un registro de tipo 'Socio'.");

            // 3) Regla de negocio: no se puede dar de baja dos veces.
            if (entidadTipo.Estado == "INACTIVO")
                throw new ReglaNegocioException("El socio ya está dado de baja.");

            // 4) Cambiar el estado y registrar la baja.
            //    'entidadTipo' viene trackeado, así que basta con modificarlo.
            entidadTipo.Estado = "INACTIVO";

            _entidadesBaja.Agregar(new EntidadBaja
            {
                Id_EntidadTipo = entidadTipo.Id_EntidadTipo,
                Fecha_Baja = DateTime.Today,
                Motivo = dto.Motivo
            });

            // 5) Confirmar ambos cambios juntos (una sola transacción).
            await _unitOfWork.GuardarCambiosAsync();
        }

        /// <summary>
        /// Modificar un socio. Solo se tocan domicilio, datos de socio y contactos:
        /// la identidad (DNI, nombre, apellido, nacimiento) NO se modifica acá.
        ///
        /// Los contactos se reemplazan por completo: se borran los actuales y se cargan
        /// los que vienen en el dto. Todo se confirma en una única transacción.
        /// </summary>
        public async Task ModificarAsync(SocioModificarDto dto)
        {
            // 1) Ubicar el socio junto con su entidad (ambos trackeados para poder modificarlos).
            var socio = await _socios.ObtenerConEntidadAsync(dto.IdSocio)
                ?? throw new RecursoNoEncontradoException("No se encontró el socio indicado.");

            var entidad = socio.Entidad
                ?? throw new InvalidOperationException("El socio no tiene una entidad asociada.");

            // 2) Datos de domicilio (viven en la entidad).
            entidad.Id_Ciudad = dto.IdCiudad!.Value;
            entidad.Calle = dto.Calle.Trim();
            entidad.Altura = dto.Altura;
            entidad.Observacion = dto.Observaciones?.Trim();

            // 3) Datos propios del socio.
            socio.Id_OS = dto.IdObraSocial;
            socio.Numero_Afiliado = dto.NumeroAfiliado?.Trim();
            socio.Plan = dto.Plan;
            socio.Sepelio = dto.Sepelio;
            socio.Cobrador = dto.Cobrador;

            // 4) Reemplazar los contactos: se eliminan los actuales y se cargan los nuevos.
            var contactosActuales = await _contactos.ObtenerPorEntidadAsync(entidad.Id_Entidad);
            foreach (var contacto in contactosActuales)
                _contactos.Eliminar(contacto);

            AgregarContactos(entidad, dto.Telefonos, dto.Emails);

            // 5) Confirmar todo junto (una sola transacción: todo o nada).
            await _unitOfWork.GuardarCambiosAsync();
        }

        /// <summary>Agrega los teléfonos y (si hay) los emails como contactos de la entidad.</summary>
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
    }
}
