using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Socios.Application.Interfaces;

namespace Socios.Api.Authentication
{
    /// <summary>
    /// BasicAuthenticationHandler intercepta requests con header Authorization: Basic
    /// Valida las credenciales y crea claims para autorización.
    /// </summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            IUsuarioRepository usuarioRepository) : base(options, logger, encoder)
        {
            _usuarioRepository = usuarioRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Si no tiene header Authorization, no intenta autenticar (NoResult = next handler)
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.NoResult();

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                // Solo maneja esquema "Basic"
                if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
                    return AuthenticateResult.NoResult();

                // Decodifica base64: "usuario:contraseña"
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                if (credentials.Length != 2)
                    return AuthenticateResult.Fail("Formato de autorización básica inválido.");

                var usuarioNombre = credentials[0];
                var password = credentials[1];

                // Obtiene el usuario de BD
                var usuario = await _usuarioRepository.GetByUsuarioNombreAsync(usuarioNombre);

                if (usuario == null)
                    return AuthenticateResult.Fail("Usuario o contraseña incorrectos.");

                // Verifica el password hasheado con BCrypt
                if (!BCrypt.Net.BCrypt.Verify(password, usuario.Password))
                    return AuthenticateResult.Fail("Usuario o contraseña incorrectos.");

                // Si llegamos aquí: credenciales válidas. Crea claims para el usuario
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id_Usuario.ToString()),
                    new Claim(ClaimTypes.Name, usuario.UsuarioNombre),
                    new Claim(ClaimTypes.Role, usuario.Rol?.RolNombre ?? "")
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (FormatException)
            {
                return AuthenticateResult.Fail("Codificación inválida en header Authorization.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error durante la autenticación");
                return AuthenticateResult.Fail("Error al autenticar.");
            }
        }
    }
}