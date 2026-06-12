namespace Socios.Domain.Entities
{
    public class Codeudor
    {
        public int Id_Codeudor { get; set; }
        public int Id_EntidadCodeudor { get; set; }
        public int Id_Entidad { get; set; }

        // Navegaciones (opcionales para consultas) 
        public Entidad? Entidad { get; set; }
        public Entidad? EntidadCodeudor { get; set; }
    }
}
