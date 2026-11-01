namespace Socios.Application.DTOs
{
    /// <summary>
    /// Una fila del informe de altas y bajas: un socio que se dio de alta y/o de baja en el
    /// período. Hay una sola fila por socio: si se dio de baja en el período, la fila muestra
    /// la baja (con su motivo); si no, el alta.
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

        /// <summary>Se dio de alta dentro del período (y se pidieron altas). Cuenta en el total de altas.</summary>
        public bool AltaEnPeriodo { get; set; }

        /// <summary>Se dio de baja dentro del período (y se pidieron bajas). Cuenta en el total de bajas.</summary>
        public bool BajaEnPeriodo { get; set; }

        public string Movimiento { get; set; } = null!; // "BAJA" si se dio de baja en el período; si no, "ALTA"
        public DateTime Fecha { get; set; }             // Fecha del movimiento que muestra la fila
    }
}
