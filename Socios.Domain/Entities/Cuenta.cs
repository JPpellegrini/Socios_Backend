namespace Socios.Domain.Entities
{
    public class Cuenta
    {
        public int Id_Cuenta { get; set; }
        public string Nombre { get; set; } = null!;
        public int NroCuenta { get; set; }
        public string Estado { get; set; } = null!;
    }
}
