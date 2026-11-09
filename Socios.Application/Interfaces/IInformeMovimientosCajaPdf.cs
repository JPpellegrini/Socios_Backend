using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Arma el PDF del reporte de ingresos y egresos.
    /// La implementación (librería de PDF, diseño de la hoja) vive en Infraestructura.
    /// </summary>
    public interface IInformeMovimientosCajaPdf
    {
        byte[] Generar(MovimientosCajaInformeDto informe);
    }
}
