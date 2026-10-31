using Microsoft.AspNetCore.Authorization;
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
        private readonly IColaboradorRepository _colaboradorRepository;

        public ColaboradoresController(IColaboradorRepository colaboradorRepository, IColaboradorUseCase colaboradores)
        {
            _colaboradorRepository = colaboradorRepository;
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


        /// <summary>
        /// Da de alta un proveedor. La orquestación (crear o reutilizar la entidad, evitar
        /// duplicados, armar el proveedor + tipo + contactos y confirmar todo en una sola
        /// transacción) vive en el caso de uso, no acá.
        ///   - Validaciones de formato → 400 (via [ApiController]).
        ///   - Reglas de negocio (ej: "ya es proveedor") → 409 (ExceptionMiddleware).
        /// </summary>
        [HttpPost("crear")]
        public async Task<IActionResult> CrearColaboradorAsync([FromBody] ColaboradorCrearDto dto)
        {
            var idColaborador = await _colaboradores.CrearAsync(dto);

            return Ok(new { idColaborador });
        }

        /// <summary>
        /// Modifica los datos editables de un colaborador (razón social, servicio prestado,
        /// domicilio y contactos). La identidad no se modifica acá.
        ///   - Validaciones de formato → 400 (via [ApiController]).
        ///   - Entidad sin colaborador   → 404 (RecursoNoEncontradoException).
        /// </summary>
        [HttpPut("modificar")]
        public async Task<IActionResult> ModificarColaboradorAsync([FromBody] ColaboradorModificarDto dto)
        {
            await _colaboradores.ModificarAsync(dto);

            return Ok(new { mensaje = "El colaborador fue modificado correctamente." });
        }

        /// <summary>
        /// Reactiva un colaborador dado de baja: recibe el Id de la entidad y pasa su estado a ACTIVO.
        ///   - Entidad sin colaborador → 404 (RecursoNoEncontradoException).
        ///   - Colaborador ya activo    → 409 (ReglaNegocioException).
        /// </summary>
        [HttpPost("reactivar")]
        public async Task<IActionResult> ReactivarColaboradorAsync([FromBody] ColaboradorReactivarDto dto)
        {
            await _colaboradores.ReactivarAsync(dto);

            return Ok(new { mensaje = "El colaborador fue reactivado correctamente." });
        }
    }
}
