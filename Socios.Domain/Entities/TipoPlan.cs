namespace Socios.Domain.Entities
{
    public class TipoPlan
    {
        public int Id_TipoPlan { get; set; }
        public int Id_TipoCuota { get; set; }
        public TipoCuota TipoCuota { get; set; } = null!;
        public int? EdadTope { get; set; }
        public string Tipo_Plan { get; set; } = null!;
    }
}