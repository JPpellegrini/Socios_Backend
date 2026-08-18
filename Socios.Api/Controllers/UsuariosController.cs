using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nichos.Infrastructure.Repositories;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/usuarios")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuariosController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarUsuarioAsync([FromQuery] UsuarioFiltroDto filtro)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioAsync(filtro);

            if (usuario == null || !usuario.Any())
                return NotFound("Este usuario no existe.");

            return Ok(usuario);
        }

        [HttpPost("alta")]
        public async Task<IActionResult> CrearUsuarioAsync([FromBody] UsuarioCrearDto dto)
        {
            var usuario = await _usuarioRepository.CrearUsuarioAsync(dto);
            return Ok(usuario);
        }

        [HttpPost("baja/{id}")]
        public async Task<IActionResult> BajaUsuario(int id)
        {
            var usuario = await _usuarioRepository.DarDeBajaUsuarioAsync(id);

            if (usuario == null)
                return NotFound(new { mensaje = "No se encontró el usuario con ese Id." });

            return Ok(new { mensaje = "Usuario dado de baja correctamente", usuario });
        }

        [HttpPut("modificar/{id}")]
        public async Task<IActionResult> ModificarUsuario(int id, [FromBody] UsuarioModificarDto dto)
        {
            var usuario = await _usuarioRepository.ModificarUsuarioAsync(id, dto);

            if (usuario == null)
                return NotFound(new { mensaje = "No se encontró el usuario con ese Id." });

            return Ok(new { mensaje = "Usuario modificado correctamente", usuario });
        }
    }
}
