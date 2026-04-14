using Microsoft.AspNetCore.Mvc;
using Socios.Api.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/me")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _usuarioRepository.GetUsuarioAsync(request.UsuarioNombre, request.Password);
            
            if (usuario == null)
                return Unauthorized();
            
            return Ok(new { usuario.Id_Usuario, usuario.UsuarioNombre, usuario.Estado, usuario.Id_Rol });
        }
    }
}
