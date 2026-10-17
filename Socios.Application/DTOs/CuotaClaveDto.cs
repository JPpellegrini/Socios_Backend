namespace Socios.Application.DTOs
{
    /// <summary>
    /// Clave lógica de una cuota ya existente, para detectar duplicados antes de generar:
    /// una cuota es única por socio (entidad) + concepto + mes + año.
    /// </summary>
    public class CuotaClaveDto
    {
        public string Concepto { get; set; } = null!;
        public int Mes { get; set; }
        public int Anio { get; set; }
    }
}
