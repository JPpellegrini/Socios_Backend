namespace Socios.Domain.Entities
{
    public class Caja
    {
        public int Id_Movimiento { get; set; }
        public int Id_CajaDiaria { get; set; }
        public EstadoCajaDiaria EstadoCajaDiaria { get; set; } = null!;
        public DateTime FechaHoraMov { get; set; }
        public string Tipo_Movimiento { get; set; } = null!;
        public int Id_Entidad { get; set; }
        public Entidad Entidad { get; set; } = null!;
        public decimal Monto { get; set; }
        public int Id_MetodoPago { get; set; }
        public MetodoPago MetodoPago { get; set; } = null!;
        public int Id_Cuenta { get; set; }
        public Cuenta Cuenta { get; set; } = null!;
        public int Id_Detmov { get; set; }
        public DetalleMovimiento DetalleMovimiento { get; set; } = null!;
        public string? Observacion { get; set; }

    }
}
