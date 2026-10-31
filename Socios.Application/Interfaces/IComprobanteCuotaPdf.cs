using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Arma el PDF de comprobantes de cuotas adeudadas. La implementación (librería de PDF,
    /// diseño de la hoja) vive en Infraestructura; el caso de uso solo le pasa los datos.
    /// </summary>
    public interface IComprobanteCuotaPdf
    {
        /// <summary>
        /// Genera un PDF con un comprobante por cada elemento, en el orden recibido.
        /// Devuelve el contenido del archivo.
        /// </summary>
        byte[] Generar(IReadOnlyList<CuotaComprobanteDto> comprobantes);
    }
}
