namespace Socios.Domain.Entities
{
    public class Cuota
    {
        public int Id_Deuda { get; set; }
        public int Id_Entidad { get; set; }
        public int Mes_Periodo { get; set; }
        public int Anio_Periodo { get; set; }
        public int Id_TipoCuota { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaHoraGeneracion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; } = null!;
        public int? Id_Movimiento { get; set; }

        public Entidad? Entidad { get; set; }
        public TipoCuota? TipoCuota { get; set; }
        public Caja? Caja { get; set; }
    }
}
