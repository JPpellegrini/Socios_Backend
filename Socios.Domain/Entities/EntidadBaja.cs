using System;

namespace Socios.Domain.Entities
{
    public class EntidadBaja
    {
        public int Id_Baja { get; set; }
        public int Id_EntidadTipo { get; set; }
        public DateTime Fecha_Baja { get; set; }
        public string Motivo { get; set; } = null!;

        public EntidadTipo? EntidadTipo { get; set; }
    }
}
