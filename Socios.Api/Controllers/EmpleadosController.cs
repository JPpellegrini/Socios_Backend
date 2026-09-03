using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/empleados")]
    [Authorize]
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        public EmpleadosController(IEmpleadoRepository empleadoRepository)
        {
            _empleadoRepository = empleadoRepository;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarEmpleadosAsync([FromQuery] EntidadFiltroBasicoDto filtro)
        {
            var empleados = await _empleadoRepository.ListarEmpleadosAsync(filtro);
            if (empleados == null || !empleados.Any())
                return NotFound(new { mensaje = $"No se encontraron empleados con el parámetro de búsqueda '{filtro.Busqueda}'." });

            return Ok(empleados);
        }

        // Endpoint para dar de baja un empleado
        [HttpPut("baja/{idEntidadTipo}")]
        public async Task<IActionResult> DarDeBajaEmpleadoAsync(int idEntidadTipo)
        {
            var empleado = await _empleadoRepository.DarDeBajaEmpleadoAsync(idEntidadTipo);

            if (empleado == null)
                return NotFound(new { mensaje = $"No se encontró un empleado activo con IdEntidadTipo = {idEntidadTipo}." });

            return Ok(new { mensaje = $"El empleado fue dado de baja correctamente." });

        }
    }
}
