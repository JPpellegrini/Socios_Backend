using Socios.Domain.Entities;

namespace Socios.Application.DTOs
{
    public class EntidadDto
    {
        public int Id_Entidad { get; set; }
        public string? CuitCuil { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? RazonSocial { get; set; }
        public string? Sexo { get; set; }
        public DateTime? Nacimiento { get; set; }
        public Ciudad? Ciudad { get; set; }
        public string? Calle { get; set; }
        public int? Altura { get; set; }
        public string? Observacion { get; set; }
    }
}
