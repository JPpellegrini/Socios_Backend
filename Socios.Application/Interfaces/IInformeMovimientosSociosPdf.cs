using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Arma el PDF del informe de altas y bajas de socios (resumen con gráfico + detalle).
    /// La implementación (librería de PDF, diseño de la hoja) vive en Infraestructura.
    /// </summary>
    public interface IInformeMovimientosSociosPdf
    {
        byte[] Generar(SocioMovimientosInformeDto informe);
    }
}
