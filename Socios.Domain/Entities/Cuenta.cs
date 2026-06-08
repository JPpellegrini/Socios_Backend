namespace Socios.Domain.Entities
{
    public class Cuenta
    {
        public int Id_Cuenta { get; set; }
        public string NombreCuenta { get; set; } = null!;
        public int NroCuenta { get; set; }
        public string Estado { get; set; } = null!;
    }
}
