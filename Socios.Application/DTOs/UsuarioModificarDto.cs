namespace Socios.Application.DTOs
{
    public class UsuarioModificarDto
    {
        public string? Password { get; set; } // opcional, se hashea si viene
        public int? Id_Rol { get; set; }      // opcional, se actualiza si viene
        public string? Estado { get; set; }   // opcional, se actualiza si viene
    }
}
