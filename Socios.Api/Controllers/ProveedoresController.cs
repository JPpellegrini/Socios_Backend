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
    }
}
