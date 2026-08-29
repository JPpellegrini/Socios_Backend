namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de salida para el buscador de socios elegibles para asignarles un nicho.
    /// Solo se listan socios ACTIVOS y con sepelio ("SI"). Incluye IdEntidad porque es
    /// el dato que consume el endpoint de asignación de nicho.
    /// </summary>
    public class SocioAsignableNichoDto
    {
        public int IdSocio { get; set; }
        public int IdEntidad { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }
    }
}
