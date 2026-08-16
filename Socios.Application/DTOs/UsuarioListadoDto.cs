using Socios.Domain.Entities;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de salida para el listado de socios (pantalla "Socios").
    /// No expone la entidad de dominio ni datos sensibles, solo lo que muestra la grilla.
    /// </summary>
    public class UsuarioListadoDto
    {
        public int Id_Usuario { get; set; }
        public string UsuarioNombre { get; set; } = null!;
        //public string Password { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string RolNombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;

    }
}
