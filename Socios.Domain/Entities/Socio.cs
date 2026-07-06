namespace Socios.Domain.Entities
{
    public class Socio
    {
        public int Id_Socio { get; set; }
        public int Id_Entidad { get; set; }
        public int? Id_OS { get; set; }
        public string? Plan { get; set; }
        public string? Sepelio { get; set; }
        public string? Cobrador { get; set; }
        public string? Numero_Afiliado { get; set; }

        public Entidad? Entidad { get; set; }
        public ObraSocial? ObraSocial { get; set; }
    }
}
