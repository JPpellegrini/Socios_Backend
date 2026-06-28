namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de salida para el listado de socios (pantalla "Socios").
    /// No expone la entidad de dominio ni datos sensibles, solo lo que muestra la grilla.
    /// </summary>
    public class SocioListadoDto
    {
        public int IdSocio { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }
        public string Estado { get; set; } = null!;
    }
}
