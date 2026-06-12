namespace Socios.Domain.Entities
{
    public class TipoCuota
    {
        public int Id_TipoCuota { get; set; }
        public string Concepto { get; set; } = null!;
        public decimal Importe { get; set; }
        public bool Tiene_EdadTope { get; set; }
        public DateTime Fecha_ultimamodif { get; set; }
    }
}