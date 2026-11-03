using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Arma el PDF del comprobante de pago de una cuota (no válido como factura). La
    /// implementación vive en Infraestructura; el caso de uso solo le pasa los datos.
    /// </summary>
    public interface IComprobantePagoPdf
    {
        /// <summary>Genera el PDF del comprobante y devuelve el contenido del archivo.</summary>
        byte[] Generar(ComprobantePagoDto comprobante);
    }
}
