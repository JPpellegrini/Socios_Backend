namespace Socios.Application.DTOs
{
    /// <summary>
    /// Datos mínimos de un socio activo para calcular sus cuotas en la generación masiva:
    /// a qué entidad se cargan, si tiene sepelio, y plan + nacimiento para el tramo.
    /// </summary>
    public class SocioCuotaDto
    {
        public int IdSocio { get; set; }
        public int IdEntidad { get; set; }
        public string? Plan { get; set; }
        public string? Sepelio { get; set; }
        public DateTime? Nacimiento { get; set; }
    }
}
