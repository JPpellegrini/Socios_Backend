using System;

namespace Socios.Domain.Entities
{
    public class Entidad
    {
        public int Id_Entidad { get; set; }
        public string Tipo { get; set; } = null!; // e.g. DNI, CUIT
        public string? Dni { get; set; }
        public string? CuitCuil { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? RazonSocial { get; set; }
        public string? Sexo { get; set; }
        public DateTime? Nacimiento { get; set; }
        public int Id_Ciudad { get; set; }
        public Ciudad? Ciudad { get; set; }
        public string? Calle { get; set; }
        public int? Altura { get; set; }
        public string? Observacion { get; set; }
        public ICollection<Contacto> Contactos { get; set; } = new List<Contacto>();
    }
}
