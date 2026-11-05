using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Arma el PDF del reporte de movimientos por entidad (proveedor o colaborador).
    /// La implementación (librería de PDF, diseño de la hoja) vive en Infraestructura.
    /// </summary>
    public interface IInformeMovimientosEntidadPdf
    {
        byte[] Generar(MovimientosEntidadInformeDto informe);
    }
}
