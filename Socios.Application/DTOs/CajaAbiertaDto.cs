namespace Socios.Application.DTOs
{
    /// <summary>
    /// Caja abierta del usuario, para las pantallas de caja (por ejemplo, el "Saldo Inicial"
    /// de la pantalla de cierre). A propósito NO incluye el efectivo esperado: el cajero
    /// declara lo que cuenta, y recién si no coincide se le informa la diferencia.
    /// </summary>
    public class CajaAbiertaDto
    {
        /// <summary>Id del registro de APERTURA en estado_cajadiaria.</summary>
        public int IdCajaDiaria { get; set; }

        public int NroCaja { get; set; }

        public DateTime FechaHoraApertura { get; set; }

        public decimal SaldoInicial { get; set; }
    }
}
