namespace Socios.Application.DTOs
{
    /// <summary>Una cuota de nicho tal como quedaría si se asigna con esa financiación.</summary>
    public class NichoCuotaSimuladaDto
    {
        /// <summary>Número de cuota (1, 2, 3...).</summary>
        public int Numero { get; set; }

        public int Mes { get; set; }
        public int Anio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal Monto { get; set; }
    }
}
