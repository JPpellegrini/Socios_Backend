using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.Interfaces;
using System.Security.Claims;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/me")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        /// <summary>
        /// Retorna la información del usuario autenticado.
        /// El header Authorization (Basic Auth) es validado automáticamente por BasicAuthenticationHandler.
        /// Si la autenticación falló, este endpoint nunca se alcanza (retorna 401).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMe()
        {
            // Obtiene el nombre de usuario del claim (lo asignó BasicAuthenticationHandler)
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(usuarioNombre))
                return Unauthorized("No se encontró información de usuario autenticado.");

            // Obtiene los datos del usuario desde BD
            var usuario = await _usuarioRepository.GetByUsuarioNombreAsync(usuarioNombre);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            // Retorna DTO sin exponer la contraseña
            return Ok(new
            {
                usuario.Id_Usuario,
                usuario.UsuarioNombre,
                usuario.Estado,
                usuario.Id_Rol,
                usuario.Rol?.RolNombre
            });
        }
    }
}
