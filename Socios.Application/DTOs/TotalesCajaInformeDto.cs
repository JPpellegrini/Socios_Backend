using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    /// <summary>Filtro del informe totalizado de ingresos y egresos.</summary>
    public class TotalesCajaFiltroDto
    {
        /// <summary>Inicio del período (inclusive).</summary>
        [Required(ErrorMessage = "La fecha desde es obligatoria.")]
        public DateTime? FechaDesde { get; set; }

        /// <summary>Fin del período (inclusive: se toma el día completo).</summary>
        [Required(ErrorMessage = "La fecha hasta es obligatoria.")]
        public DateTime? FechaHasta { get; set; }

        /// <summary>"TODOS" (default), "INGRESOS" o "EGRESOS".</summary>
        public string? TipoMovimiento { get; set; } = MovimientosCajaFiltroDto.TodosLosTipos;
    }

    /// <summary>Fila tal como sale de la consulta agrupada: una actividad con un tipo de movimiento.</summary>
    public class TotalCajaAgrupadoDto
    {
        public int IdActividad { get; set; }
        public string Actividad { get; set; } = null!;
        /// <summary>"INGRESO", "EGRESO" u otro valor si el dato está mal cargado.</summary>
        public string TipoMovimiento { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>Una línea del informe totalizado: el total de una actividad dentro de su sección.</summary>
    public class TotalActividadDto
    {
        public int IdActividad { get; set; }
        public string Actividad { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        /// <summary>Participación sobre el total de la sección (0 a 1).</summary>
        public decimal Porcentaje { get; set; }
    }

    /// <summary>
    /// Informe totalizado de ingresos y egresos del período: una sección de ingresos y otra de
    /// egresos con el total por actividad, el total de cada sección y el resultado del período.
    /// </summary>
    public class TotalesCajaInformeDto
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        /// <summary>"TODOS", "INGRESOS" o "EGRESOS".</summary>
        public string TipoMovimiento { get; set; } = MovimientosCajaFiltroDto.TodosLosTipos;

        public bool IncluyeIngresos => TipoMovimiento != MovimientosCajaFiltroDto.Egresos;
        public bool IncluyeEgresos => TipoMovimiento != MovimientosCajaFiltroDto.Ingresos;

        /// <summary>Ingresos por actividad, de mayor a menor (vacía si el filtro es solo egresos).</summary>
        public List<TotalActividadDto> Ingresos { get; set; } = new();
        /// <summary>Egresos por actividad, de mayor a menor (vacía si el filtro es solo ingresos).</summary>
        public List<TotalActividadDto> Egresos { get; set; } = new();

        public decimal TotalIngresos => Ingresos.Sum(i => i.Total);
        public decimal TotalEgresos => Egresos.Sum(e => e.Total);
        public int CantidadIngresos => Ingresos.Sum(i => i.Cantidad);
        public int CantidadEgresos => Egresos.Sum(e => e.Cantidad);

        /// <summary>Ingresos menos egresos. Solo tiene sentido con el filtro "TODOS".</summary>
        public decimal? Resultado => IncluyeIngresos && IncluyeEgresos ? TotalIngresos - TotalEgresos : null;

        /// <summary>Movimientos del período con un tipo distinto de INGRESO o EGRESO (no suman).</summary>
        public int MovimientosSinTipoValido { get; set; }

        public bool SinMovimientos => CantidadIngresos == 0 && CantidadEgresos == 0;

        /// <summary>Arma el informe a partir de los totales agrupados de la consulta.</summary>
        public static TotalesCajaInformeDto Armar(
            DateTime fechaDesde, DateTime fechaHasta, string tipoMovimiento, List<TotalCajaAgrupadoDto> agrupados)
        {
            var informe = new TotalesCajaInformeDto
            {
                FechaDesde = fechaDesde.Date,
                FechaHasta = fechaHasta.Date,
                TipoMovimiento = tipoMovimiento,
                MovimientosSinTipoValido = agrupados
                    .Where(a => a.TipoMovimiento != MovimientosCajaInformeDto.TipoIngreso
                             && a.TipoMovimiento != MovimientosCajaInformeDto.TipoEgreso)
                    .Sum(a => a.Cantidad)
            };

            if (informe.IncluyeIngresos)
                informe.Ingresos = Seccion(agrupados, MovimientosCajaInformeDto.TipoIngreso);
            if (informe.IncluyeEgresos)
                informe.Egresos = Seccion(agrupados, MovimientosCajaInformeDto.TipoEgreso);

            return informe;
        }

        private static List<TotalActividadDto> Seccion(List<TotalCajaAgrupadoDto> agrupados, string tipo)
        {
            var filas = agrupados.Where(a => a.TipoMovimiento == tipo).ToList();
            var total = filas.Sum(f => f.Total);

            return filas
                .Select(f => new TotalActividadDto
                {
                    IdActividad = f.IdActividad,
                    Actividad = f.Actividad,
                    Cantidad = f.Cantidad,
                    Total = f.Total,
                    Porcentaje = total == 0 ? 0 : f.Total / total
                })
                .OrderByDescending(f => f.Total)
                .ThenBy(f => f.Actividad)
                .ToList();
        }
    }
}
