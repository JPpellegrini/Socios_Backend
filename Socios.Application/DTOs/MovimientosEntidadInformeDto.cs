namespace Socios.Application.DTOs
{
    /// <summary>
    /// Entidad para el selector del reporte y para el encabezado del informe.
    /// Incluye activas e inactivas: los movimientos de una entidad dada de baja también se consultan.
    /// </summary>
    public class EntidadMovimientosDto
    {
        public int IdEntidad { get; set; }
        public string? Dni { get; set; }
        public string? CuitCuil { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? RazonSocial { get; set; }
        public string? Prestacion { get; set; }
        public string? Estado { get; set; }
    }

    /// <summary>Movimiento de caja tal como sale de la base (montos siempre positivos).</summary>
    public class MovimientoEntidadDto
    {
        public int IdMovimiento { get; set; }
        public DateTime FechaHoraMovimiento { get; set; }
        public string? Concepto { get; set; }
        public string? TipoMovimiento { get; set; }
        public decimal Monto { get; set; }
        public string? MetodoPago { get; set; }
        public string? Cuenta { get; set; }
        public int NroCuenta { get; set; }
        public string? Observacion { get; set; }

        /// <summary>
        /// Importe a mostrar: positivo si el movimiento va en el sentido esperado para el tipo de
        /// entidad (ingreso para colaborador, egreso para proveedor); negativo si va en sentido
        /// contrario (por ejemplo, una devolución), para que reste del total.
        /// </summary>
        public decimal Importe { get; set; }
    }

    /// <summary>Reporte de movimientos de caja de una entidad en un período.</summary>
    public class MovimientosEntidadInformeDto
    {
        /// <summary>"PROVEEDOR" o "COLABORADOR".</summary>
        public string Tipo { get; set; } = null!;
        /// <summary>Título de la columna de importe: "Ingreso" (colaborador) o "Egreso" (proveedor).</summary>
        public string TituloImporte { get; set; } = null!;
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public EntidadMovimientosDto Entidad { get; set; } = new();
        public List<MovimientoEntidadDto> Movimientos { get; set; } = new();

        public int Cantidad => Movimientos.Count;

        /// <summary>Total neto de la columna importe (los movimientos en sentido contrario restan).</summary>
        public decimal Total => Movimientos.Sum(m => m.Importe);

        /// <summary>Indica si hay movimientos en sentido contrario al esperado (importes negativos).</summary>
        public bool TieneMovimientosEnContra => Movimientos.Any(m => m.Importe < 0);

        /// <summary>Arma el informe calculando el signo de cada importe según el tipo de entidad.</summary>
        public static MovimientosEntidadInformeDto Armar(
            TipoEntidadMovimientos tipo,
            DateTime fechaDesde,
            DateTime fechaHasta,
            EntidadMovimientosDto entidad,
            List<MovimientoEntidadDto> movimientos)
        {
            foreach (var m in movimientos)
            {
                var esperado = string.Equals(m.TipoMovimiento?.Trim(), tipo.TipoMovimientoEsperado,
                    StringComparison.OrdinalIgnoreCase);
                m.Importe = esperado ? m.Monto : -m.Monto;
            }

            return new MovimientosEntidadInformeDto
            {
                Tipo = tipo.Codigo,
                TituloImporte = tipo.TituloImporte,
                FechaDesde = fechaDesde.Date,
                FechaHasta = fechaHasta.Date,
                Entidad = entidad,
                Movimientos = movimientos
            };
        }
    }
}
