namespace Socios.Domain.Entities
{
    public class Contacto
    {
        public int Id_Contacto { get; set; }
        public string Tipo { get; set; } = null!;
        public string ContactoValor { get; set; } = null!;
        public int Id_Entidad { get; set; }

        public Entidad? Entidad { get; set; }
    }
}
