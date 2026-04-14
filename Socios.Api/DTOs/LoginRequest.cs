namespace Socios.Api.DTOs
{
    public class LoginRequest
    {
        public string UsuarioNombre { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
