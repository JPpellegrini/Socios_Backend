namespace Socios.Domain.Entities
{
    public class Cuenta
    {
        public int Id_Cuenta { get; set; }
        public string NombreCuenta { get; set; } = null!;
        public int NroCuenta { get; set; } = 0;
        public string Estado { get; set; } = null!;

        /// <summary>
        /// Dinero que había en la cuenta al comienzo de <see cref="FechaSaldoInicial"/> (ej. el
        /// saldo del extracto bancario a esa fecha). Se carga una vez: el sistema solo conoce los
        /// movimientos que se registran en él, no lo que había antes.
        /// </summary>
        public decimal SaldoInicial { get; set; }

        /// <summary>
        /// Fecha desde la que rige el saldo inicial: el saldo de la cuenta es el inicial más los
        /// movimientos desde ese día inclusive. Null = el saldo inicial es anterior a todos los
        /// movimientos (se suman todos).
        /// </summary>
        public DateTime? FechaSaldoInicial { get; set; }
    }
}
