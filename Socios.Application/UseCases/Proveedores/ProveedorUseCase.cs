using System.Text.RegularExpressions;
using Socios.Application.DTOs;
using Socios.Application.Exceptions;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Application.UseCases.Proveedores
{
    /// <summary>
    /// Reúne los casos de uso del proveedor. Cada método coordina QUÉ hay que hacer,
    /// pero NO habla directamente con la base de datos: eso se lo delega a los
    /// repositorios y a la unidad de trabajo. Cada capa en lo suyo.
    /// </summary>
    public class ProveedorUseCase : IProveedorUseCase
    {
        private readonly IEntidadRepository _entidades;
        private readonly IProveedorRepository _proveedores;
        private readonly IEntidadTipoRepository _entidadesTipo;
        private readonly IContactoRepository _contactos;
        private readonly IPrestacionRepository _prestaciones;
        private readonly IUnitOfWork _unitOfWork;

        // Id del tipo de entidad "Proveedor" (catálogo TiposEntidad).
        private const int IdTipoProveedor = 2;

        public ProveedorUseCase(
            IEntidadRepository entidades,
            IProveedorRepository proveedores,
            IEntidadTipoRepository entidadesTipo,
            IContactoRepository contactos,
            IPrestacionRepository prestaciones,
            IUnitOfWork unitOfWork)
        {
            _entidades = entidades;
            _proveedores = proveedores;
            _entidadesTipo = entidadesTipo;
            _contactos = contactos;
            _prestaciones = prestaciones;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Alta de proveedor. Reglas:
        ///   - El documento puede ser DNI (7-8 dígitos) o CUIT/CUIL (11 dígitos).
        ///   - Si la entidad NO existe → la crea.
        ///   - Si YA existe y todavía NO es proveedor → la reutiliza (no la duplica).
        ///   - Si YA existe y YA es proveedor → corta con un error de negocio.
        /// Al final crea el Proveedor, su tipo "Proveedor" (ACTIVO) y los contactos, y
        /// confirma TODO junto en una única transacción.
        /// </summary>
        public async Task<int> CrearAsync(ProveedorCrearDto dto)
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
                    RazonSocial = dto.RazonSocial.Trim(),
                    Id_Ciudad = dto.IdCiudad!.Value,
                    Calle = dto.Calle.Trim(),
                    Altura = dto.Altura,
                    Observacion = dto.Observaciones?.Trim()
                };

                _entidades.Agregar(entidad);
            }
            else if (await _proveedores.EsProveedorAsync(entidad.Id_Entidad))
            {
                // Existe y ya es proveedor → no se puede dar de alta dos veces.
                throw new ReglaNegocioException("Ya existe un proveedor registrado con ese documento.");
            }

            // Colgamos de esa entidad el proveedor, su tipo (ACTIVO) y los contactos.
            // Al compartir la misma instancia de 'entidad', EF resuelve solo las claves foráneas.
            var proveedor = new Proveedor
            {
                Entidad = entidad,
                Id_Prestacion = dto.IdPrestacion!.Value
            };

            _proveedores.Agregar(proveedor);

            _entidadesTipo.Agregar(new EntidadTipo
            {
                Entidad = entidad,
                Id_Tipo = IdTipoProveedor,
                Fecha_Alta = DateTime.Today,
                Estado = "ACTIVO"
            });

            AgregarContactos(entidad, dto.Telefonos, dto.Emails);

            // Recién acá se confirma TODO junto (una sola transacción: todo o nada).
            await _unitOfWork.GuardarCambiosAsync();

            // El id se completa después de guardar.
            return proveedor.Id_Proveedor;
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
