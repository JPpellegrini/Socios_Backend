namespace Socios.Domain.Entities
{
    public class Colaborador
    {
        public int Id_Colaborador { get; set; }
        public int Id_Prestacion { get; set; }
        public int Id_Entidad { get; set; }

        public Prestacion? Prestacion { get; set; }
        public Entidad? Entidad { get; set; }
    }
}
