using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Arma el PDF del informe totalizado de ingresos y egresos.
    /// La implementación (librería de PDF, diseño de la hoja) vive en Infraestructura.
    /// </summary>
    public interface IInformeTotalesCajaPdf
    {
        byte[] Generar(TotalesCajaInformeDto informe);
    }
}
