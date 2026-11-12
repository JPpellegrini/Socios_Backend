using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.UseCases.Prestaciones;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/prestaciones")]
    [Authorize]
    public class PrestacionesController : ControllerBase
    {
        private readonly IPrestacionUseCase _prestaciones;

        public PrestacionesController(IPrestacionUseCase prestaciones)
        {
            _prestaciones = prestaciones;
        }

        /// <summary>
        /// Listado de prestaciones ordenado por nombre (para el desplegable del alta y la
        /// modificación de proveedores y colaboradores). Sin prestaciones → 200 con lista vacía.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ListarAsync()
        {
            return Ok(await _prestaciones.ListarAsync());
        }

        /// <summary>
        /// Alta de una prestación. El nombre se guarda en mayúsculas y no puede repetirse
        /// (sin distinguir mayúsculas ni tildes).
        ///   - Nombre vacío o fuera de 3 a 100 caracteres → 400 (via [ApiController]).
        ///   - Ya existe una prestación con ese nombre    → 409 (ExceptionMiddleware).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CrearAsync([FromBody] PrestacionCrearDto dto)
        {
            var prestacion = await _prestaciones.CrearAsync(dto);
            return Ok(prestacion);
        }
    }
}
