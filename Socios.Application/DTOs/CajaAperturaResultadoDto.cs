namespace Socios.Application.DTOs
{
    /// <summary>
    /// Resultado de la apertura de caja. Además de la caja abierta, informa el saldo con el que
    /// el usuario cerró su caja anterior y la diferencia con el efectivo declarado ahora, para
    /// que un faltante o sobrante entre un día y otro quede a la vista (no impide abrir).
    /// </summary>
    public class CajaAperturaResultadoDto
    {
        public string Mensaje { get; set; } = null!;

        /// <summary>Id del registro de apertura en estado_cajadiaria.</summary>
        public int IdCajaDiaria { get; set; }

        /// <summary>Número de caja asignado. El cierre de esta caja se registra con el mismo número.</summary>
        public int NroCaja { get; set; }

        public DateTime FechaHoraApertura { get; set; }

        public decimal SaldoInicial { get; set; }

        /// <summary>Número de la última caja que cerró el usuario. Null si nunca cerró una.</summary>
        public int? NroCajaUltimoCierre { get; set; }

        public DateTime? FechaHoraUltimoCierre { get; set; }

        /// <summary>Efectivo con el que el usuario cerró su última caja. Null si nunca cerró una.</summary>
        public decimal? SaldoUltimoCierre { get; set; }

        /// <summary>
        /// SaldoInicial − SaldoUltimoCierre: positivo = sobrante, negativo = faltante, 0 = coincide.
        /// Null si no hay cierre anterior con qué comparar.
        /// </summary>
        public decimal? Diferencia { get; set; }
    }
}
