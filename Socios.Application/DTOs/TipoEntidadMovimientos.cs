namespace Socios.Application.DTOs
{
    /// <summary>
    /// Cómo se arma el reporte de movimientos para cada tipo de entidad: qué tipo de entidad
    /// listar, qué conceptos (detalles de movimiento) le corresponden y en qué sentido se
    /// espera el dinero.
    ///
    /// IMPORTANTE: la relación concepto → tipo de entidad está fija acá porque la base todavía
    /// no la guarda. Si se agrega un detalle de movimiento nuevo para proveedores o colaboradores
    /// (por ejemplo "HONORARIOS"), hay que sumar su Id en <see cref="IdsDetalleMovimiento"/>;
    /// si no, sus movimientos no aparecen en el reporte.
    /// </summary>
    public sealed class TipoEntidadMovimientos
    {
        public const string TipoIngreso = "INGRESO";
        public const string TipoEgreso = "EGRESO";

        /// <summary>Colaborador: aporta dinero a la institución (ingresos por colaboración).</summary>
        public static readonly TipoEntidadMovimientos Colaborador = new(
            codigo: "COLABORADOR",
            descripcion: "Colaborador",
            idTipoEntidad: 3,
            idsDetalleMovimiento: new[] { 4 },     // 4 = COLABORACIÓN
            tipoMovimientoEsperado: TipoIngreso,
            tituloImporte: "Ingreso");

        /// <summary>Proveedor: la institución le paga (egresos por pago a proveedor).</summary>
        public static readonly TipoEntidadMovimientos Proveedor = new(
            codigo: "PROVEEDOR",
            descripcion: "Proveedor",
            idTipoEntidad: 2,
            idsDetalleMovimiento: new[] { 5 },     // 5 = PAGO A PROVEEDOR
            tipoMovimientoEsperado: TipoEgreso,
            tituloImporte: "Egreso");

        private TipoEntidadMovimientos(string codigo, string descripcion, int idTipoEntidad,
            int[] idsDetalleMovimiento, string tipoMovimientoEsperado, string tituloImporte)
        {
            Codigo = codigo;
            Descripcion = descripcion;
            IdTipoEntidad = idTipoEntidad;
            IdsDetalleMovimiento = idsDetalleMovimiento;
            TipoMovimientoEsperado = tipoMovimientoEsperado;
            TituloImporte = tituloImporte;
        }

        /// <summary>Valor que llega en el filtro: "PROVEEDOR" o "COLABORADOR".</summary>
        public string Codigo { get; }
        public string Descripcion { get; }
        /// <summary>Id_Tipo en tipos_entidad / entidad_tipos.</summary>
        public int IdTipoEntidad { get; }
        /// <summary>Detalles de movimiento (conceptos) que se listan para este tipo.</summary>
        public IReadOnlyList<int> IdsDetalleMovimiento { get; }
        /// <summary>Sentido normal del dinero. Un movimiento en sentido contrario resta.</summary>
        public string TipoMovimientoEsperado { get; }
        /// <summary>Título de la columna de importe y del total: "Ingreso" o "Egreso".</summary>
        public string TituloImporte { get; }

        /// <summary>Devuelve la configuración del tipo indicado, o null si no es válido.</summary>
        public static TipoEntidadMovimientos? Obtener(string? codigo) =>
            codigo?.Trim().ToUpperInvariant() switch
            {
                "COLABORADOR" => Colaborador,
                "PROVEEDOR" => Proveedor,
                _ => null
            };
    }
}
