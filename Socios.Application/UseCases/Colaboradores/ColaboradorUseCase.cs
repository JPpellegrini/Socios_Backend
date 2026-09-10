using Socios.Application.DTOs;
using Socios.Application.Exceptions;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using System.Text.RegularExpressions;

namespace Socios.Application.UseCases.Colaboradores
{
    public class ColaboradorUseCase : IColaboradorUseCase
    {
        private readonly IColaboradorRepository _colaboradores;
        private readonly IContactoRepository _contactos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntidadRepository _entidades;
        private readonly IEntidadTipoRepository _entidadesTipo;
        private readonly IPrestacionRepository _prestaciones;
        public ColaboradorUseCase(
            IEntidadRepository entidades,
            IColaboradorRepository colaboradores,
            IContactoRepository contactos,
            IEntidadTipoRepository entidadesTipo,
            IPrestacionRepository prestaciones,
            IUnitOfWork unitOfWork)
        {
            _entidades = entidades;
            _colaboradores = colaboradores;
            _contactos = contactos;
            _entidadesTipo = entidadesTipo;
            _prestaciones = prestaciones;
            _unitOfWork = unitOfWork;
        }

        private const int IdTipoColaborador = 3;

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

        /// <summary>
        /// Alta de colaborador. Reglas:
        ///   - El documento puede ser DNI (7-8 dígitos) o CUIT/CUIL (11 dígitos).
        ///   - Si la entidad NO existe → la crea.
        ///   - Si YA existe y todavía NO es colaborador → la reutiliza (no la duplica).
        ///   - Si YA existe y YA es proveedor → corta con un error de negocio.
        /// Al final crea el Colaborador, su tipo "Colaborador" (ACTIVO) y los contactos, y
        /// confirma TODO junto en una única transacción.
        /// </summary>
        public async Task<int> CrearAsync(ColaboradorCrearDto dto)
        {
            var tipoDocumento = dto.TipoDocumento.Trim().ToUpperInvariant();
            var documento = dto.Documento.Trim();

            // Validación de largo según el tipo de documento (regla de negocio).
            if (tipoDocumento == "DNI" && !Regex.IsMatch(documento, @"^\d{7,8}$"))
                throw new ReglaNegocioException("El DNI debe tener 7 u 8 dígitos.");

            if (tipoDocumento == "CUIT" && !Regex.IsMatch(documento, @"^\d{11}$"))
                throw new ReglaNegocioException("El CUIT/CUIL debe tener 11 dígitos.");

            // El servicio prestado (prestación) tiene que existir.
            if (!await _prestaciones.ExisteAsync(dto.IdPrestacion!.Value))
                throw new ReglaNegocioException($"No existe una prestación con el Id {dto.IdPrestacion} para asignar como servicio prestado.");

            // ¿Ya existe una entidad con ese documento? Se busca por DNI o por CUIT según el tipo.
            var entidad = tipoDocumento == "DNI"
                ? await _entidades.ObtenerPorDniAsync(documento)
                : await _entidades.ObtenerPorCuitCuilAsync(documento);

            if (entidad is null)
            {
                // No existe → la creamos a partir de los datos del formulario.
                entidad = new Entidad
                {
                    Tipo = tipoDocumento,
                    Dni = tipoDocumento == "DNI" ? documento : null,
                    CuitCuil = tipoDocumento == "CUIT" ? documento : null,
                    Nombre = dto.Nombre.Trim().ToUpperInvariant(),
                    Apellido = dto.Apellido.Trim().ToUpperInvariant(),
                    RazonSocial = dto.RazonSocial?.Trim().ToUpperInvariant(),
                    Nacimiento = dto.FechaNacimiento,
                    Id_Ciudad = dto.IdCiudad!.Value,
                    Calle = dto.Calle.Trim().ToUpperInvariant(),
                    Altura = dto.Altura,
                    Observacion = dto.Observaciones?.Trim().ToUpperInvariant()
                };

                _entidades.Agregar(entidad);
            }
            else if (await _colaboradores.EsColaboradorAsync(entidad.Id_Entidad))
            {
                // Existe y ya es colaborador → no se puede dar de alta dos veces.
                throw new ReglaNegocioException("Ya existe un colaborador registrado con ese documento.");
            }

            // Colgamos de esa entidad el colaborador, su tipo (ACTIVO) y los contactos.
            // Al compartir la misma instancia de 'entidad', EF resuelve solo las claves foráneas.
            var colaborador = new Colaborador
            {
                Entidad = entidad,
                Id_Prestacion = dto.IdPrestacion!.Value
            };

            _colaboradores.Agregar(colaborador);

            _entidadesTipo.Agregar(new EntidadTipo
            {
                Entidad = entidad,
                Id_Tipo = IdTipoColaborador,
                Fecha_Alta = DateTime.Today,
                Estado = "ACTIVO"
            });

            AgregarContactos(entidad, dto.Telefonos, dto.Emails);

            // Recién acá se confirma TODO junto (una sola transacción: todo o nada).
            await _unitOfWork.GuardarCambiosAsync();

            // El id se completa después de guardar.
            return colaborador.Id_Colaborador;
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