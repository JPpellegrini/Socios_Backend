using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Application.UseCases.Empleados;

namespace Socios.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoUseCase _empleados;

        public EmpleadosController(IEmpleadoUseCase empleados)
        {
            _empleados = empleados;
        }

        /// <summary>
        /// Alta de empleado
        /// </summary>
        [HttpPost("alta")]
        public async Task<IActionResult> CrearEmpleadoAsync([FromBody] EmpleadoCrearDto dto)
        {
            var idEntidad = await _empleados.CrearAsync(dto);

            return Ok( new {id = idEntidad , mensaje = $"Empleado {dto.Nombre} {dto.Apellido} creado correctamente." });
        }

        /// <summary>
        /// Visualizar un empleado por IdEntidad
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerEmpleadoAsync(int id)
        {
            var empleado = await _empleados.VisualizarAsync(id);

            if (empleado is null)
                return NotFound(new { mensaje = $"No se encontró el empleado con IdEntidad {id}." });

            return Ok(empleado);
        }

        [HttpPut("baja/{idEntidadTipo}")]
        public async Task<IActionResult> DarDeBajaEmpleadoAsync(int idEntidadTipo, [FromBody] EmpleadoBajaDto dto)
        {
            var resultado = await _empleados.BajaAsync(idEntidadTipo, dto.Motivo);

            if (!resultado)
                return NotFound(new { mensaje = $"No se encontró el empleado con IdEntidadTipo {idEntidadTipo} o ya estaba inactivo." });

            return Ok(new { mensaje = $"Empleado dado de baja correctamente con motivo: {dto.Motivo}" });
        }

        /// <summary>
        /// Modificar empleado
        /// </summary>
        [HttpPut("modificar")]
        public async Task<IActionResult> ModificarEmpleadoAsync([FromBody] EmpleadoModificarDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _empleados.ModificarAsync(dto);

            return Ok(new { mensaje = $"Empleado {dto.IdEntidad} modificado correctamente." });
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarEmpleadoAsync([FromQuery] string? busqueda)
        {
            var empleados = await _empleados.BuscarAsync(busqueda);

            if (empleados == null || !empleados.Any())
                return NotFound(new { mensaje = "No se encontraron empleados con ese filtro." });

            return Ok(empleados);
        }
    }
}
