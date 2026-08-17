namespace Socios.Domain.Entities
{
    /// <summary>
    /// Un proveedor es una entidad (persona o empresa) que presta un servicio.
    /// Se modela igual que el colaborador: cuelga de una <see cref="Entidad"/> y
    /// referencia la <see cref="Prestacion"/> que representa el servicio prestado.
    /// El estado (ACTIVO / INACTIVO) vive en la fila de <see cref="EntidadTipo"/> de tipo "Proveedor".
    /// </summary>
    public class Proveedor
    {
        public int Id_Proveedor { get; set; }
        public int Id_Prestacion { get; set; }
        public int Id_Entidad { get; set; }

        // Navegaciones (opcionales para consultas)
        public Prestacion? Prestacion { get; set; }
        public Entidad? Entidad { get; set; }
    }
}
