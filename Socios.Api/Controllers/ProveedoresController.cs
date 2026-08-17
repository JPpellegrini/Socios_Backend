using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Application.UseCases.Proveedores;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/proveedores")]
    [Authorize]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly IProveedorUseCase _proveedorUseCase;

        public ProveedoresController(IProveedorRepository proveedorRepository, IProveedorUseCase proveedorUseCase)
        {
            _proveedorRepository = proveedorRepository;
            _proveedorUseCase = proveedorUseCase;
        }

        /// <summary>
        /// Lista los proveedores para la pantalla "Proveedores".
        /// Los filtros viajan en el query string (?Busqueda=&CuitCuil=&IncluirInactivos=).
        /// Filtros (todos opcionales, combinados con AND):
        ///   - Busqueda: coincidencia parcial por Nombre, Apellido o Razón Social.
        ///   - CuitCuil: coincidencia parcial por CUIT o CUIL.
        ///   - IncluirInactivos: false (default) solo ACTIVOS; true incluye INACTIVOS (ver todo).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarProveedoresAsync([FromQuery] ProveedorFiltroDto filtro)
        {
            var proveedores = await _proveedorRepository.BuscarAsync(filtro);

            return Ok(proveedores);
        }

        /// <summary>
        /// Da de alta un proveedor. La orquestación (crear o reutilizar la entidad, evitar
        /// duplicados, armar el proveedor + tipo + contactos y confirmar todo en una sola
        /// transacción) vive en el caso de uso, no acá.
        ///   - Validaciones de formato → 400 (via [ApiController]).
        ///   - Reglas de negocio (ej: "ya es proveedor") → 409 (ExceptionMiddleware).
        /// </summary>
        [HttpPost("crear")]
        public async Task<IActionResult> CrearProveedorAsync([FromBody] ProveedorCrearDto dto)
        {
            var idProveedor = await _proveedorUseCase.CrearAsync(dto);

            return Ok(new { idProveedor });
        }

        /// <summary>
        /// Da de baja un proveedor: recibe el Id de la entidad y pasa su estado a INACTIVO.
        ///   - Entidad sin proveedor → 404 (RecursoNoEncontradoException).
        ///   - Proveedor ya inactivo → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("baja")]
        public async Task<IActionResult> DarDeBajaProveedorAsync([FromBody] ProveedorBajaDto dto)
        {
            await _proveedorUseCase.BajaAsync(dto);

            return Ok(new { mensaje = "El proveedor fue dado de baja correctamente." });
        }

        /// <summary>
        /// Modifica los datos editables de un proveedor (razón social, servicio prestado,
        /// domicilio y contactos). La identidad no se modifica acá.
        ///   - Validaciones de formato → 400 (via [ApiController]).
        ///   - Entidad sin proveedor   → 404 (RecursoNoEncontradoException).
        /// </summary>
        [HttpPut("modificar")]
        public async Task<IActionResult> ModificarProveedorAsync([FromBody] ProveedorModificarDto dto)
        {
            await _proveedorUseCase.ModificarAsync(dto);

            return Ok(new { mensaje = "El proveedor fue modificado correctamente." });
        }

        /// <summary>
        /// Reactiva un proveedor dado de baja: recibe el Id de la entidad y pasa su estado a ACTIVO.
        ///   - Entidad sin proveedor → 404 (RecursoNoEncontradoException).
        ///   - Proveedor ya activo    → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("reactivar")]
        public async Task<IActionResult> ReactivarProveedorAsync([FromBody] ProveedorReactivarDto dto)
        {
            await _proveedorUseCase.ReactivarAsync(dto);

            return Ok(new { mensaje = "El proveedor fue reactivado correctamente." });
        }

        /// <summary>
        /// Trae un proveedor por el Id de su entidad, con todos sus datos (identidad, domicilio,
        /// servicio prestado, estado, fechas y contactos). Devuelve 404 si esa entidad no es proveedor.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> VisualizarProveedorAsync(int id)
        {
            var proveedor = await _proveedorUseCase.VisualizarAsync(id);

            if (proveedor is null)
                return NotFound(new { mensaje = "No se encontró el proveedor solicitado." });

            return Ok(proveedor);
        }
    }
}
