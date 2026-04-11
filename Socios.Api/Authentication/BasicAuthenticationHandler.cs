using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Api.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IUsuarioRepository usuarioRepository) : base(options, logger, encoder)
        {
            _usuarioRepository = usuarioRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.NoResult();

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
                    return AuthenticateResult.NoResult();

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                if (credentials.Length != 2)
                    return AuthenticateResult.Fail("Invalid Authorization header format.");

                var email = credentials[0];
                var password = credentials[1];

                var usuario = await _usuarioRepository.GetByEmailAsync(email);
                if (usuario == null)
                    return AuthenticateResult.Fail("Invalid credentials.");

                var hasher = new PasswordHasher<Usuario>();
                var verifyResult = hasher.VerifyHashedPassword(usuario, usuario.Password, password);
                if (verifyResult == PasswordVerificationResult.Failed)
                    return AuthenticateResult.Fail("Invalid credentials.");

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id_Usuario.ToString()),
                    new Claim(ClaimTypes.Name, usuario.UsuarioNombre)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (FormatException)
            {
                return AuthenticateResult.Fail("Invalid Authorization header encoding.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error authenticating");
                return AuthenticateResult.Fail("An error occurred while authenticating.");
            }
        }
    }
}
