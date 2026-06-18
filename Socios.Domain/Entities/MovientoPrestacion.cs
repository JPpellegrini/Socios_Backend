namespace Socios.Domain.Entities
{
    public class MovimientoPrestacion
    {
        public int Id_MovimientoPrestacion { get; set; }
        public int Id_Movimiento { get; set; }
        public int Id_Prestacion { get; set; }
        public Caja? Caja { get; set; }
        public Prestacion? Prestacion { get; set; }
    }
}
