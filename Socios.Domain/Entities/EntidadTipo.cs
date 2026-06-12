using System;

namespace Socios.Domain.Entities
{
    public class EntidadTipo
    {
        public int Id_EntidadTipo { get; set; }
        public int Id_Entidad { get; set; }
        public int Id_Tipo { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public string Estado { get; set; } = null!;

        public Entidad? Entidad { get; set; }
        public TipoEntidad? TipoEntidad { get; set; }
    }
}
