using Socios.Domain.Entities;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de salida para el listado de socios (pantalla "Socios").
    /// No expone la entidad de dominio ni datos sensibles, solo lo que muestra la grilla.
    /// </summary>
    public class NichoListadoDto
    {
        public int? Id_Nicho { get; set; }
        public int? Id_Entidad { get; set; }
        public string? EntidadNombre { get; set; }
        public string Sector { get; set; } = null!;
        public string NroNicho { get; set; } = null!;
        public decimal? ValorNicho { get; set; }
        public decimal? ValorLapida { get; set; }
        public string ConLapida { get; set; } = null!;
        public string Ocupado { get; set; } = null!;
        public int? Cuotas { get; set; }
        public decimal? InteresMensual { get; set; }
    }
}
