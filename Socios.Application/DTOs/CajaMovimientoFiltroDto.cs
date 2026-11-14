using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>
    /// Filtros de la consulta de movimientos de caja (GET api/v1/caja/movimientos). Todos son
    /// opcionales y se combinan con AND. Sin usuario ni caja se ven los movimientos de TODAS las
    /// cajas (la vista general); con usuario, solo los registrados en las cajas de ese usuario
    /// (su caja chica), incluidas las transferencias que él registró.
    /// </summary>
    public class CajaMovimientoFiltroDto : IValidatableObject
    {
        /// <summary>
        /// Desde esta fecha inclusive (se ignora la hora). Sin ninguna de las dos fechas: hoy,
        /// SALVO que se pida un NroCaja (ahí no se filtra por fecha y viene la caja completa,
        /// aunque haya abierto otro día). Con una sola fecha: ese día.
        /// </summary>
        public DateTime? FechaDesde { get; set; }

        /// <summary>Hasta esta fecha inclusive (todo el día). Misma regla que <see cref="FechaDesde"/>.</summary>
        public DateTime? FechaHasta { get; set; }

        /// <summary>Usuario dueño de la caja donde se registró el movimiento. Sin valor: todos.</summary>
        public int? IdUsuario { get; set; }

        /// <summary>
        /// Número de caja (una sesión de apertura/cierre): muestra exactamente lo que cuenta el
        /// cierre de esa caja. Sin valor: todas.
        /// </summary>
        public int? NroCaja { get; set; }

        /// <summary>Cuenta del movimiento (EFECTIVO, banco...). Sin valor: todas.</summary>
        public int? IdCuenta { get; set; }

        /// <summary>
        /// Texto de entidad: contiene (sin distinguir mayúsculas) en nombre, apellido, razón
        /// social o DNI. Vacío no filtra.
        /// </summary>
        public string? Entidad { get; set; }

        /// <summary>
        /// Texto de descripción: contiene (sin distinguir mayúsculas) en el motivo (detalle del
        /// movimiento, ej. "CUOTA SOCIO") o en la observación. Vacío no filtra.
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// INGRESO o EGRESO. "TODOS" (o no enviar el parámetro) no filtra.
        /// El null en la lista es obligatorio: [AllowedValues] rechaza null si no está.
        /// </summary>
        [AllowedValues(null, "TODOS", "INGRESO", "EGRESO",
            ErrorMessage = "El tipo debe ser TODOS, INGRESO o EGRESO.")]
        public string? Tipo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaDesde.HasValue && FechaHasta.HasValue && FechaDesde.Value.Date > FechaHasta.Value.Date)
                yield return new ValidationResult(
                    "La fecha desde no puede ser posterior a la fecha hasta.",
                    new[] { nameof(FechaDesde) });
        }
    }
}
