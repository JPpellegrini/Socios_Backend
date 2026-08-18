namespace Socios.Application.DTOs
{
    public class UsuarioCrearDto
    {
        public string UsuarioNombre { get; set; } = null!;
        public string Password { get; set; } = null!; // texto plano, se va a hashear
        public string Estado { get; set; } = "Activo"; // por defecto activo
        public int Id_Rol { get; set; }
        
    }
}
