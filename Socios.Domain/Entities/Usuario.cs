namespace Socios.Domain.Entities
{
    public class Usuario
    {
        public int Id_Usuario { get; set; }
        public string UsuarioNombre { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public int Id_Rol { get; set; }
        public Rol Rol { get; set; } = null!;
    }
}
