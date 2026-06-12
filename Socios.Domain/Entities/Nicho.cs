namespace Socios.Domain.Entities
{
    public class Nicho
    {
        public int Id_Nicho { get; set; }
        public int Id_Entidad { get; set; }
        public Entidad? Entidad { get; set; }
        public string Sector { get; set; } = null!;
        public string Nro_Nicho { get; set; } = null!;
        public decimal Valor_Nicho { get; set; }
        public decimal Valor_Lapida { get; set; }
        public string ConLapida { get; set; } = null!;
        public string Ocupado { get; set; } = null!;
        public int? Cuotas { get; set; }
        public decimal? InteresMensual { get; set; }
    }
}