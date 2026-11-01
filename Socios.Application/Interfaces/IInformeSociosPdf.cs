using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Arma el PDF del informe de socios. La implementación (librería de PDF, diseño de la
    /// hoja) vive en Infraestructura; el controlador solo le pasa los datos y el filtro usado.
    /// </summary>
    public interface IInformeSociosPdf
    {
        /// <summary>
        /// Genera un PDF con una fila por socio, en el orden recibido. El filtro se usa solo
        /// para mostrar en el encabezado qué criterios se aplicaron.
        /// </summary>
        byte[] Generar(IReadOnlyList<SocioInformeDto> socios, SocioInformeFiltroDto filtro);
    }
}
