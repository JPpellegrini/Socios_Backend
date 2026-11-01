namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de salida del informe de altas y bajas: el detalle de movimientos y el resumen
    /// (totales y bajas por motivo) con el que el frontend o el PDF arman el gráfico.
    /// </summary>
    public class SocioMovimientosInformeDto
    {
        /// <summary>
        /// Motivos de baja de socio admitidos (ver SocioBajaDto). Siempre se informan en el
        /// resumen, aunque tengan 0, para que el gráfico muestre las mismas barras en todo período.
        /// </summary>
        public static readonly string[] MotivosBaja = { "MORA", "FALLECIMIENTO", "RENUNCIA" };

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string Tipo { get; set; } = null!; // TODOS, ALTAS o BAJAS

        public int Altas { get; set; }
        public int Bajas { get; set; }
        public int VariacionNeta => Altas - Bajas;
        public List<BajaPorMotivoDto> BajasPorMotivo { get; set; } = new();

        public List<SocioMovimientoDto> Movimientos { get; set; } = new();

        /// <summary>
        /// Arma el informe a partir de los movimientos ya filtrados: calcula los totales y
        /// agrupa las bajas por motivo (de mayor a menor cantidad).
        /// </summary>
        public static SocioMovimientosInformeDto Armar(SocioMovimientoFiltroDto filtro, List<SocioMovimientoDto> movimientos)
        {
            var bajas = movimientos.Where(m => m.Movimiento == "BAJA").ToList();

            // Motivos conocidos (aunque tengan 0) más cualquier otro que venga en los datos.
            var motivos = MotivosBaja
                .Union(bajas.Select(b => b.MotivoBaja ?? "SIN MOTIVO"))
                .Select(motivo => new BajaPorMotivoDto
                {
                    Motivo = motivo,
                    Cantidad = bajas.Count(b => (b.MotivoBaja ?? "SIN MOTIVO") == motivo)
                })
                .OrderByDescending(x => x.Cantidad)
                .ToList();

            return new SocioMovimientosInformeDto
            {
                FechaDesde = filtro.FechaDesde!.Value.Date,
                FechaHasta = filtro.FechaHasta!.Value.Date,
                Tipo = filtro.Tipo,
                Altas = movimientos.Count(m => m.Movimiento == "ALTA"),
                Bajas = bajas.Count,
                BajasPorMotivo = motivos,
                Movimientos = movimientos
            };
        }
    }

    public class BajaPorMotivoDto
    {
        public string Motivo { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}
