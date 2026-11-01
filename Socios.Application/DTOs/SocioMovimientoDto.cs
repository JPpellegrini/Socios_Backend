namespace Socios.Application.DTOs
{
    /// <summary>
    /// Una fila del informe de altas y bajas: un movimiento (alta o baja) de un socio.
    /// Un socio que se dio de alta y de baja dentro del mismo período aparece dos veces,
    /// una por cada movimiento.
    /// </summary>
    public class SocioMovimientoDto
    {
        public int IdSocio { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Calle { get; set; } = null!;
        public string Altura { get; set; } = null!;
        public string Ciudad { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? ObraSocial { get; set; }
        public int? Edad { get; set; } // null si no hay fecha de nacimiento cargada
        public bool Sepelio { get; set; }
        public bool Nicho { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string? MotivoBaja { get; set; } // MORA, FALLECIMIENTO o RENUNCIA (null si no tiene baja)

        public string Movimiento { get; set; } = null!; // "ALTA" o "BAJA"
        public DateTime Fecha { get; set; }             // Fecha del movimiento (alta o baja)
    }
}
