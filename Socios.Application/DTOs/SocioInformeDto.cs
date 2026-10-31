namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de salida para el informe de socios.
    /// Contiene únicamente los datos que se muestran en el reporte.
    /// </summary>
    public class SocioInformeDto
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Calle { get; set; } = null!;
        public string Altura { get; set; } = null!;
        public string Ciudad { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Mail { get; set; }
        public string? ObraSocial { get; set; }
        public int Edad { get; set; }
        public bool Sepelio { get; set; }
        public bool Nicho { get; set; }
        public string Estado { get; set; } = null!;
        public string TipoSocio { get; set; } = null!; // "Sepelio" o "Adherente"
    }
}
