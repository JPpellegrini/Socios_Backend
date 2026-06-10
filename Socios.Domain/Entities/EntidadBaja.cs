using System;

namespace Socios.Domain.Entities
{
    public class EntidadBaja
    {
        public int Id_Baja { get; set; }
        public int Id_Entidad { get; set; }
        public int Id_Tipo { get; set; }
        public DateTime Fecha_Baja { get; set; }
        public string Motivo { get; set; } = null!;

        // Navegaciones
        public Entidad? Entidad { get; set; }
        public TipoEntidad? TipoEntidad { get; set; }

        //validar en codigo que exista en la tabla Entidad_Tipo cuando se vaya a grabar.
    }
}
