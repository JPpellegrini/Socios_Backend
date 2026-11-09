namespace Socios.Application.DTOs
{
    /// <summary>
    /// Filtro del informe de deudas. Por defecto busca sin fechas (todas las cuotas adeudadas).
    /// </summary>
    public class DeudasFiltroDto
    {
        /// <summary>Entidad del socio. Null = todos los socios.</summary>
        public int? IdEntidad { get; set; }

        /// <summary>True (default): ignora las fechas y trae todas las deudas.</summary>
        public bool SinFechas { get; set; } = true;

        /// <summary>Inicio del período por fecha de vencimiento (inclusive). Obligatoria si SinFechas = false.</summary>
        public DateTime? FechaDesde { get; set; }

        /// <summary>Fin del período por fecha de vencimiento (inclusive). Obligatoria si SinFechas = false.</summary>
        public DateTime? FechaHasta { get; set; }

        /// <summary>
        /// Estado guardado de la cuota: "TODAS" (default), "VENCIDAS" (estado VENCIDA) o
        /// "PENDIENTES" (estado PENDIENTE, incluidas las que ya pasaron su vencimiento sin que
        /// nadie las marcara como VENCIDA).
        /// </summary>
        public string? EstadoCuota { get; set; } = Todas;

        public const string Todas = "TODAS";
        public const string Vencidas = "VENCIDAS";
        public const string Pendientes = "PENDIENTES";

        /// <summary>
        /// Estados guardados que corresponden al filtro, o null si el valor no es válido.
        /// Normaliza EstadoCuota (mayúsculas, sin espacios; vacío = TODAS).
        /// </summary>
        public string[]? EstadosAFiltrar()
        {
            EstadoCuota = string.IsNullOrWhiteSpace(EstadoCuota) ? Todas : EstadoCuota.Trim().ToUpperInvariant();
            return EstadoCuota switch
            {
                Todas => new[] { "PENDIENTE", "VENCIDA" },
                Vencidas => new[] { "VENCIDA" },
                Pendientes => new[] { "PENDIENTE" },
                _ => null
            };
        }
    }

    /// <summary>Una cuota adeudada, con los datos del socio al que pertenece.</summary>
    public class DeudaCuotaDto : SocioCuotaCuentaDto
    {
        public int IdEntidad { get; set; }
        public int IdSocio { get; set; }
        public string? Apellido { get; set; }
        public string? Nombre { get; set; }
        public string? Dni { get; set; }

        /// <summary>Días transcurridos desde el vencimiento a la fecha de corte (0 si todavía no venció).</summary>
        public int DiasAtraso { get; set; }
    }

    /// <summary>Subtotal de deuda de un tipo de cuota o de un socio.</summary>
    public class SubtotalDeudaDto
    {
        public string Descripcion { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>
    /// Informe de deudas: cuotas adeudadas (PENDIENTE o VENCIDA) de un socio o de todos, con el
    /// monto que se cobraría hoy (una VENCIDA se cobra al importe vigente, salvo NICHO), los
    /// días de atraso y los totales por tipo de cuota y por socio.
    /// </summary>
    public class DeudasInformeDto
    {
        /// <summary>Fecha a la que se calculan los días de atraso.</summary>
        public DateTime FechaCorte { get; set; }
        public bool SinFechas { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        /// <summary>"TODAS", "VENCIDAS" o "PENDIENTES".</summary>
        public string EstadoCuota { get; set; } = DeudasFiltroDto.Todas;

        /// <summary>Socio filtrado, o null si el informe es de todos los socios.</summary>
        public SocioSeleccionDto? Socio { get; set; }

        public List<DeudaCuotaDto> Cuotas { get; set; } = new();

        public int Cantidad => Cuotas.Count;
        public decimal Total => Cuotas.Sum(c => c.Monto);
        public int CantidadSocios => Cuotas.Select(c => c.IdEntidad).Distinct().Count();

        /// <summary>Cuotas ya vencidas a la fecha de corte (estén marcadas VENCIDA o sigan PENDIENTE).</summary>
        public int CantidadVencidas => Cuotas.Count(c => c.DiasAtraso > 0);
        public decimal TotalVencido => Cuotas.Where(c => c.DiasAtraso > 0).Sum(c => c.Monto);

        public bool TieneMontosActualizados => Cuotas.Any(c => c.MontoActualizado);

        /// <summary>Totales por tipo de cuota, de mayor a menor.</summary>
        public List<SubtotalDeudaDto> PorTipo => Cuotas
            .GroupBy(c => c.TipoCuota)
            .Select(g => new SubtotalDeudaDto { Descripcion = g.Key, Cantidad = g.Count(), Total = g.Sum(c => c.Monto) })
            .OrderByDescending(s => s.Total)
            .ThenBy(s => s.Descripcion)
            .ToList();

        /// <summary>Arma el informe: calcula el monto a cobrar y los días de atraso de cada cuota.</summary>
        public static DeudasInformeDto Armar(DeudasFiltroDto filtro, SocioSeleccionDto? socio,
            List<DeudaCuotaDto> cuotas, DateTime fechaCorte)
        {
            SocioEstadoCuentaDto.CalcularMontos(cuotas);
            foreach (var c in cuotas)
                c.DiasAtraso = Math.Max(0, (fechaCorte.Date - c.FechaVencimiento.Date).Days);

            return new DeudasInformeDto
            {
                FechaCorte = fechaCorte.Date,
                SinFechas = filtro.SinFechas,
                FechaDesde = filtro.SinFechas ? null : filtro.FechaDesde?.Date,
                FechaHasta = filtro.SinFechas ? null : filtro.FechaHasta?.Date,
                EstadoCuota = filtro.EstadoCuota ?? DeudasFiltroDto.Todas,
                Socio = socio,
                Cuotas = cuotas
            };
        }
    }
}
