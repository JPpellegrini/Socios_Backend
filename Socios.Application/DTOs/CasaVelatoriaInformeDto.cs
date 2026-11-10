namespace Socios.Application.DTOs
{
    /// <summary>Un socio habilitado para el servicio de sepelio a la fecha de emisión.</summary>
    public class SocioHabilitadoSepelioDto
    {
        public int IdSocio { get; set; }
        public int IdEntidad { get; set; }
        public string? Apellido { get; set; }
        public string? Nombre { get; set; }
        public string? Dni { get; set; }
        public DateTime? Nacimiento { get; set; }
        /// <summary>Edad a la fecha de emisión (null si no tiene fecha de nacimiento).</summary>
        public int? Edad { get; set; }
        public string? Plan { get; set; }
        public bool Sepelio { get; set; }
        public bool Nicho { get; set; }
        /// <summary>Meses con al menos una cuota vencida e impaga a la fecha de emisión.</summary>
        public int MesesAdeudados { get; set; }
    }

    /// <summary>
    /// Listado para la casa velatoria: socios ACTIVOS con sepelio y/o nicho que, a la fecha de
    /// emisión, no adeudan más de <see cref="MaximoMesesAdeudados"/> meses. Es una foto de ese día:
    /// después de la emisión la situación de un socio puede cambiar.
    /// </summary>
    public class CasaVelatoriaInformeDto
    {
        /// <summary>Condición de la casa velatoria: con más meses adeudados no accede al servicio.</summary>
        public const int MaximoMesesAdeudados = 3;

        public DateTime FechaEmision { get; set; }
        public int MaximoMeses => MaximoMesesAdeudados;
        public List<SocioHabilitadoSepelioDto> Socios { get; set; } = new();
        public int Cantidad => Socios.Count;

        /// <summary>Completa la edad de cada socio a la fecha de emisión.</summary>
        public static CasaVelatoriaInformeDto Armar(List<SocioHabilitadoSepelioDto> socios, DateTime fechaEmision)
        {
            var hoy = fechaEmision.Date;
            foreach (var s in socios)
                s.Edad = s.Nacimiento is { } n
                    ? hoy.Year - n.Year - (n.Date > hoy.AddYears(-(hoy.Year - n.Year)) ? 1 : 0)
                    : null;

            return new CasaVelatoriaInformeDto { FechaEmision = hoy, Socios = socios };
        }
    }
}
