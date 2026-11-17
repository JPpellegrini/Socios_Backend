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
        /// Listado de prestaciones ordenado por nombre.
        ///   - estado: ACTIVA (por defecto, para los desplegables de proveedores y colaboradores),
        ///     INACTIVA o TODOS (para la pantalla de gestión).
        ///   - tipo: COLABORACION o PRESTACION; vacío = ambos.
        ///   - nombre: parte del nombre, sin distinguir mayúsculas ni tildes ("cardio" → CARDIOLOGÍA).
        /// Valores inválidos → 400. Sin prestaciones → 200 con lista vacía.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ListarAsync([FromQuery] PrestacionFiltroDto filtro)
        {
            return Ok(await _prestaciones.ListarAsync(filtro));
        }

        /// <summary>
        /// Alta de una prestación con nombre y tipo (COLABORACION o PRESTACION); se crea ACTIVA.
        /// El nombre se guarda en mayúsculas y no puede repetirse (sin distinguir mayúsculas ni
        /// tildes, contando también las inactivas).
        ///   - Nombre fuera de 3 a 100 caracteres o tipo inválido → 400 (via [ApiController]).
        ///   - Ya existe una prestación con ese nombre            → 409 (ExceptionMiddleware).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CrearAsync([FromBody] PrestacionCrearDto dto)
        {
            var prestacion = await _prestaciones.CrearAsync(dto);
            return Ok(prestacion);
        }

        /// <summary>
        /// Modificación del nombre y el tipo de una prestación (mismas reglas que el alta).
        /// El estado no se cambia acá: para eso están la baja y la reactivación.
        ///   - Nombre o tipo inválido            → 400.
        ///   - No existe la prestación           → 404.
        ///   - Otra prestación ya usa ese nombre → 409.
        /// </summary>
        [HttpPut("modificar/{idPrestacion:int}")]
        public async Task<IActionResult> ModificarAsync(int idPrestacion, [FromBody] PrestacionModificarDto dto)
        {
            var prestacion = await _prestaciones.ModificarAsync(idPrestacion, dto);
            return Ok(new { mensaje = "Prestación modificada correctamente.", prestacion });
        }

        /// <summary>
        /// Baja lógica: pasa a INACTIVA y deja de ofrecerse para asignar; quienes ya la tienen
        /// la conservan.
        ///   - No existe la prestación → 404.
        ///   - Ya estaba inactiva      → 409.
        /// </summary>
        [HttpPost("baja/{idPrestacion:int}")]
        public async Task<IActionResult> DarDeBajaAsync(int idPrestacion)
        {
            var prestacion = await _prestaciones.DarDeBajaAsync(idPrestacion);
            return Ok(new { mensaje = "Prestación dada de baja correctamente.", prestacion });
        }

        /// <summary>
        /// Reactiva una prestación inactiva (pasa a ACTIVA).
        ///   - No existe la prestación → 404.
        ///   - Ya estaba activa        → 409.
        /// </summary>
        [HttpPost("reactivar/{idPrestacion:int}")]
        public async Task<IActionResult> ReactivarAsync(int idPrestacion)
        {
            var prestacion = await _prestaciones.ReactivarAsync(idPrestacion);
            return Ok(new { mensaje = "Prestación reactivada correctamente.", prestacion });
        }
    }
}
