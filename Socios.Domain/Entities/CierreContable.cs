namespace Socios.Domain.Entities
{
    public class CierreContable
    {
        public int Id_Cierre { get; set; }
        public string Tipo { get; set; } = null!;
        public int? Mes_Periodo { get; set; }
        public int? Anio_Periodo { get; set; }
        public DateTime Fechor { get; set; }
        public int Id_Usuario { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
