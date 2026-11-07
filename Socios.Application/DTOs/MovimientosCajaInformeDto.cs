using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>Filtro del reporte de ingresos y egresos (movimientos de caja de la institución).</summary>
    public class MovimientosCajaFiltroDto
    {
        /// <summary>Inicio del período (inclusive).</summary>
        [Required(ErrorMessage = "La fecha desde es obligatoria.")]
        public DateTime? FechaDesde { get; set; }

        /// <summary>Fin del período (inclusive: se toma el día completo).</summary>
        [Required(ErrorMessage = "La fecha hasta es obligatoria.")]
        public DateTime? FechaHasta { get; set; }

        /// <summary>"TODOS" (default), "INGRESOS" o "EGRESOS".</summary>
        public string? TipoMovimiento { get; set; } = TodosLosTipos;

        /// <summary>Id del detalle de movimiento (tipo de actividad). Null = todas.</summary>
        public int? IdActividad { get; set; }

        public const string TodosLosTipos = "TODOS";
        public const string Ingresos = "INGRESOS";
        public const string Egresos = "EGRESOS";
    }

    /// <summary>Tipo de actividad (detalle de movimiento) para el desplegable del filtro.</summary>
    public class ActividadCajaDto
    {
        public int IdActividad { get; set; }
        public string Nombre { get; set; } = null!;
    }

    /// <summary>Un movimiento de caja del reporte.</summary>
    public class MovimientoCajaDto
    {
        public int IdMovimiento { get; set; }
        public DateTime FechaHoraMovimiento { get; set; }
        /// <summary>"INGRESO" o "EGRESO", normalizado a mayúsculas.</summary>
        public string TipoMovimiento { get; set; } = null!;
        public int IdEntidad { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? RazonSocial { get; set; }
        public decimal Monto { get; set; }
        public string? MetodoPago { get; set; }
        public int IdActividad { get; set; }
        public string? Actividad { get; set; }
        public string? Observacion { get; set; }

        public bool EsIngreso => TipoMovimiento == MovimientosCajaInformeDto.TipoIngreso;
        public bool EsEgreso => TipoMovimiento == MovimientosCajaInformeDto.TipoEgreso;
    }

    /// <summary>Subtotal de ingresos y egresos de una actividad (o de un mes).</summary>
    public class SubtotalCajaDto
    {
        public string Descripcion { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal Ingresos { get; set; }
        public decimal Egresos { get; set; }
        public decimal Saldo => Ingresos - Egresos;
    }

    /// <summary>
    /// Reporte de ingresos y egresos del período: detalle de movimientos, totales y subtotales
    /// por actividad y por mes (para la lectura anual).
    /// </summary>
    public class MovimientosCajaInformeDto
    {
        public const string TipoIngreso = "INGRESO";
        public const string TipoEgreso = "EGRESO";

        private static readonly string[] Meses =
        {
            "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO",
            "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
        };

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        /// <summary>"TODOS", "INGRESOS" o "EGRESOS".</summary>
        public string TipoMovimiento { get; set; } = MovimientosCajaFiltroDto.TodosLosTipos;
        /// <summary>Nombre de la actividad filtrada, o null si son todas.</summary>
        public string? Actividad { get; set; }
        public List<MovimientoCajaDto> Movimientos { get; set; } = new();

        public int Cantidad => Movimientos.Count;
        public decimal TotalIngresos => Movimientos.Where(m => m.EsIngreso).Sum(m => m.Monto);
        public decimal TotalEgresos => Movimientos.Where(m => m.EsEgreso).Sum(m => m.Monto);
        /// <summary>Ingresos menos egresos del período (con los filtros aplicados).</summary>
        public decimal Saldo => TotalIngresos - TotalEgresos;

        /// <summary>Subtotales por actividad, de mayor a menor importe movido (ingresos + egresos).</summary>
        public List<SubtotalCajaDto> PorActividad => Movimientos
            .GroupBy(m => m.Actividad ?? "-")
            .Select(g => Subtotal(g.Key, g))
            .OrderByDescending(s => s.Ingresos + s.Egresos)
            .ThenBy(s => s.Descripcion)
            .ToList();

        /// <summary>Subtotales por mes, en orden cronológico.</summary>
        public List<SubtotalCajaDto> PorMes => Movimientos
            .GroupBy(m => new { m.FechaHoraMovimiento.Year, m.FechaHoraMovimiento.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => Subtotal($"{Meses[g.Key.Month - 1]} {g.Key.Year}", g))
            .ToList();

        /// <summary>Movimientos con un tipo que no es INGRESO ni EGRESO (no suman en los totales).</summary>
        public int MovimientosSinTipoValido => Movimientos.Count(m => !m.EsIngreso && !m.EsEgreso);

        private static SubtotalCajaDto Subtotal(string descripcion, IEnumerable<MovimientoCajaDto> movimientos)
        {
            var lista = movimientos.ToList();
            return new SubtotalCajaDto
            {
                Descripcion = descripcion,
                Cantidad = lista.Count,
                Ingresos = lista.Where(m => m.EsIngreso).Sum(m => m.Monto),
                Egresos = lista.Where(m => m.EsEgreso).Sum(m => m.Monto)
            };
        }
    }
}
