namespace Socios.Application.DTOs
{
    /// <summary>
    /// Cuánto dinero hay en una cuenta según el sistema:
    /// Saldo = SaldoInicial + Ingresos − Egresos (movimientos desde FechaSaldoInicial).
    /// Es lo REGISTRADO: el efectivo real se verifica contando y los bancos, contra el extracto.
    /// </summary>
    public class CuentaSaldoDto
    {
        public int IdCuenta { get; set; }

        public string NombreCuenta { get; set; } = null!;

        public int NroCuenta { get; set; }

        public string Estado { get; set; } = null!;

        public decimal SaldoInicial { get; set; }

        /// <summary>Desde cuándo rige el saldo inicial. Null = desde el primer movimiento.</summary>
        public DateTime? FechaSaldoInicial { get; set; }

        public decimal Ingresos { get; set; }

        public decimal Egresos { get; set; }

        public decimal Saldo { get; set; }
    }
}
