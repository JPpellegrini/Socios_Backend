using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Application.UseCases.Colaboradores;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/colaboradores")]
    public class ColaboradoresController : ControllerBase
    {
        private readonly IColaboradorUseCase _colaboradores;

        public ColaboradoresController(IColaboradorUseCase colaboradores)
        {
            _colaboradores = colaboradores;
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarAsync([FromQuery] string? busqueda)
        {
            var colaboradores = await _colaboradores.BuscarAsync(busqueda);

            if (colaboradores == null || !colaboradores.Any())
                return NotFound(new { mensaje = "No se encontraron colaboradores con ese filtro." });

            return Ok(colaboradores);
        }


        [HttpPut("baja/{idEntidadTipo}")]
        public async Task<IActionResult> DarDeBajaEmpleadoAsync(int idEntidadTipo, [FromBody] ColaboradorBajaDto dto)
        {
            var resultado = await _colaboradores.BajaAsync(idEntidadTipo, dto.Motivo);

            if (!resultado)
                return NotFound(new { mensaje = $"No se encontró el colaborador con IdEntidadTipo {idEntidadTipo} o ya estaba inactivo." });

            return Ok(new { mensaje = $"Colaborador dado de baja correctamente con motivo: {dto.Motivo}" });
        }
    }
}
