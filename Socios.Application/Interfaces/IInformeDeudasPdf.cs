using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Arma el PDF del informe de deudas de socios.
    /// La implementación (librería de PDF, diseño de la hoja) vive en Infraestructura.
    /// </summary>
    public interface IInformeDeudasPdf
    {
        byte[] Generar(DeudasInformeDto informe);
    }
}
