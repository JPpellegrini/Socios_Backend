using Socios.Domain.Entities;

namespace Socios.Application.DTOs
{
    public class ColaboradorDto
    {
        public int Id_Entidad { get; set; }
        public string? Dni { get; set; }
        public string? CuitCuil { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Sexo { get; set; }
        public DateTime? Nacimiento { get; set; }
        public Ciudad? Ciudad { get; set; }
        public string? Calle { get; set; }
        public int? Altura { get; set; }
        public string? Observacion { get; set; }
        public List<string> Telefonos { get; set; } = new();
        public List<string> Emails { get; set; } = new();
    }
}
