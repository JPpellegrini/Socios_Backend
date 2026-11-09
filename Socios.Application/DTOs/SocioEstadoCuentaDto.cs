using System.ComponentModel.DataAnnotations;
using Socios.Application.UseCases.Pagos;

namespace Socios.Application.DTOs
{
    /// <summary>Filtro del estado de cuenta del socio. La entidad viaja en la ruta.</summary>
    public class SocioEstadoCuentaFiltroDto
    {
        /// <summary>Inicio del período por fecha de vencimiento de la cuota (inclusive).</summary>
        [Required(ErrorMessage = "La fecha desde es obligatoria.")]
        public DateTime? FechaDesde { get; set; }

        /// <summary>Fin del período por fecha de vencimiento de la cuota (inclusive).</summary>
        [Required(ErrorMessage = "La fecha hasta es obligatoria.")]
        public DateTime? FechaHasta { get; set; }
    }

    /// <summary>Socio para el selector del filtro y el encabezado del reporte.</summary>
    public class SocioSeleccionDto
    {
        public int IdEntidad { get; set; }
        public int IdSocio { get; set; }
        public string? Dni { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Estado { get; set; }
    }

    /// <summary>Una cuota del socio, con los datos del pago si está paga.</summary>
    public class SocioCuotaCuentaDto
    {
        public int IdCuota { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public string TipoCuota { get; set; } = null!;
        public DateTime FechaVencimiento { get; set; }

        /// <summary>
        /// Estado a mostrar: el guardado, salvo una PENDIENTE cuyo vencimiento ya pasó, que se
        /// informa VENCIDA (misma regla que el cobro; ver MontoCuotaACobrar). Lo calcula CalcularMontos.
        /// </summary>
        public string Estado { get; set; } = null!;

        /// <summary>Estado tal como está guardado en la base.</summary>
        public string EstadoGuardado { get; set; } = null!;

        /// <summary>Monto guardado en la cuota (en las pagas, lo efectivamente cobrado).</summary>
        public decimal MontoCuota { get; set; }

        /// <summary>Importe vigente del tipo de cuota (para recalcular las vencidas).</summary>
        public decimal ImporteVigente { get; set; }

        /// <summary>
        /// Monto que se informa: el cobrado si está paga, o lo que se cobraría hoy si está
        /// adeudada (una VENCIDA se cobra al importe vigente, salvo NICHO).
        /// </summary>
        public decimal Monto { get; set; }

        /// <summary>True si es una vencida cuyo monto se actualizó al importe vigente.</summary>
        public bool MontoActualizado { get; set; }

        public int? IdMovimiento { get; set; }
        public DateTime? FechaHoraPago { get; set; }
        public string? MetodoPago { get; set; }
        public string? Cuenta { get; set; }
        public int? NroCuenta { get; set; }
        public string? Observacion { get; set; }

        public bool EstaPaga => Estado == SocioEstadoCuentaDto.EstadoPagado;
    }

    /// <summary>
    /// Estado de cuenta de un socio: sus cuotas (excepto las anuladas) con vencimiento en el
    /// período, con lo pagado y lo adeudado, más la deuda total del socio a la fecha.
    /// </summary>
    public class SocioEstadoCuentaDto
    {
        public const string EstadoPagado = "PAGADO";
        public const string EstadoPendiente = "PENDIENTE";

        public SocioSeleccionDto Socio { get; set; } = new();
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public List<SocioCuotaCuentaDto> Cuotas { get; set; } = new();

        public int Cantidad => Cuotas.Count;

        /// <summary>Suma de la columna monto (pagado + adeudado del período).</summary>
        public decimal Total => Cuotas.Sum(c => c.Monto);

        /// <summary>Cobrado en las cuotas del período.</summary>
        public decimal TotalPagado => Cuotas.Where(c => c.EstaPaga).Sum(c => c.Monto);

        /// <summary>Adeudado en las cuotas del período (pendientes y vencidas, al monto a cobrar).</summary>
        public decimal TotalAdeudado => Cuotas.Where(c => !c.EstaPaga).Sum(c => c.Monto);

        /// <summary>
        /// Deuda total del socio a la fecha, sin importar el período elegido: evita que una
        /// deuda vieja quede oculta por el filtro de fechas.
        /// </summary>
        public decimal DeudaTotal { get; set; }

        public int CuotasAdeudadasTotal { get; set; }

        public bool TieneMontosActualizados => Cuotas.Any(c => c.MontoActualizado);

        /// <summary>
        /// Calcula, con las reglas del cobro (MontoCuotaACobrar) a la fecha 'hoy', el estado a
        /// mostrar y el monto a informar de cada cuota. Se puede llamar una sola vez por cuota:
        /// toma como estado guardado el que trae la consulta.
        /// </summary>
        public static void CalcularMontos(IEnumerable<SocioCuotaCuentaDto> cuotas, DateTime hoy)
        {
            foreach (var c in cuotas)
            {
                c.EstadoGuardado = c.Estado;
                c.Estado = MontoCuotaACobrar.EstadoEfectivo(c.EstadoGuardado, c.FechaVencimiento, hoy);

                if (c.EstaPaga)
                {
                    c.Monto = c.MontoCuota;
                    c.MontoActualizado = false;
                    continue;
                }

                c.Monto = MontoCuotaACobrar.Calcular(c.EstadoGuardado, c.TipoCuota, c.FechaVencimiento,
                    c.MontoCuota, c.ImporteVigente, hoy);
                c.MontoActualizado = c.Monto != c.MontoCuota;
            }
        }
    }
}
