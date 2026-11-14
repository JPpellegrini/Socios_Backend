namespace Socios.Application.DTOs
{
    /// <summary>
    /// Fila de la grilla de movimientos de caja. Trae más datos de los que muestra la pantalla
    /// (ids, número de caja, nombre separado de la entidad) para que el frontend tome lo que necesite.
    /// </summary>
    public class CajaMovimientoListadoDto
    {
        public int IdMovimiento { get; set; }

        public DateTime FechaHoraMovimiento { get; set; }

        /// <summary>Usuario dueño de la caja donde se registró.</summary>
        public int IdUsuario { get; set; }
        public string Usuario { get; set; } = null!;

        /// <summary>Caja (sesión) donde se registró.</summary>
        public int IdCajaDiaria { get; set; }
        public int NroCaja { get; set; }

        /// <summary>"INGRESO" o "EGRESO" (normalizado a mayúsculas).</summary>
        public string Tipo { get; set; } = null!;

        public int IdEntidad { get; set; }

        /// <summary>Nombre para mostrar: razón social o "APELLIDO NOMBRE".</summary>
        public string Entidad { get; set; } = null!;

        /// <summary>Detalle del movimiento (ej. CUOTA SOCIO, TRANSFERENCIA).</summary>
        public int IdMotivo { get; set; }
        public string Motivo { get; set; } = null!;

        public string? Observacion { get; set; }

        public string MetodoPago { get; set; } = null!;

        /// <summary>Monto del movimiento (siempre positivo).</summary>
        public decimal Monto { get; set; }

        /// <summary>Monto con signo para la grilla: positivo si es ingreso, negativo si es egreso.</summary>
        public decimal Importe { get; set; }

        public int IdCuenta { get; set; }
        public string Cuenta { get; set; } = null!;
    }
}
