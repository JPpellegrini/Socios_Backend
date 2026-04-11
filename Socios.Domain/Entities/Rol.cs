namespace Socios.Domain.Entities
{
    public class Rol
    {
        public int Id_Rol { get; set; }
        public string RolNombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
