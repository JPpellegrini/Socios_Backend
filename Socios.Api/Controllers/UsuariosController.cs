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
    }
}
