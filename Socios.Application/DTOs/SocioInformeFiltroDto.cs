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

        // Las tres categorías se combinan con OR: se listan los socios que cumplan
        // alguna de las marcadas. Al menos una debe estar marcada.

        /// <summary>
        /// Incluye socios adherentes (sin sepelio).
        /// </summary>
        public bool IncluirAdherentes { get; set; } = true;

        /// <summary>
        /// Incluye socios con sepelio que no tienen nicho asignado.
        /// </summary>
        public bool IncluirSepelio { get; set; } = true;

        /// <summary>
        /// Incluye socios que tienen un nicho asignado.
        /// </summary>
        public bool IncluirNicho { get; set; } = true;
    }
}
