namespace Socios.Domain.Entities
{
    public class EstadoCajaDiaria
    {
        public int Id_CajaDiaria { get; set; }
        public int Id_Usuario { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public DateTime FechaHora { get; set; }
        public decimal? Saldo { get; set; }
    }
}
