using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/proveedores")]
    [Authorize]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorRepository _proveedorRepository;

        public ProveedoresController(IProveedorRepository proveedorRepository)
        {
            _proveedorRepository = proveedorRepository;
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
    }
}
