namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de entrada para filtrar el informe de socios.
    /// </summary>
    public class SocioInformeFiltroDto
    {
        /// <summary>
        /// Estado de socio: "ACTIVO", "INACTIVO" o "TODOS".
        /// </summary>
        public string Estado { get; set; } = "TODOS";

        /// <summary>
        /// Indica si se incluyen socios adherentes.
        /// </summary>
        public bool IncluirAdherentes { get; set; } = true;

        /// <summary>
        /// Indica si se incluyen socios con sepelio.
        /// </summary>
        public bool IncluirSepelio { get; set; } = true;

        /// <summary>
        /// Indica si se incluyen socios con nicho.
        /// </summary>
        public bool IncluirNicho { get; set; } = true;
    }
}
