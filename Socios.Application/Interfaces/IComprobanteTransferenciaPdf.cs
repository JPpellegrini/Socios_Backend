using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Arma el PDF del comprobante de una transferencia entre cuentas del Centro. La
    /// implementación vive en Infraestructura; el caso de uso solo le pasa los datos.
    /// </summary>
    public interface IComprobanteTransferenciaPdf
    {
        /// <summary>Genera el PDF del comprobante y devuelve el contenido del archivo.</summary>
        byte[] Generar(ComprobanteTransferenciaDto comprobante);
    }
}
