using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Arma el PDF del listado de socios habilitados para la casa velatoria.
    /// La implementación (librería de PDF, diseño de la hoja) vive en Infraestructura.
    /// </summary>
    public interface IInformeCasaVelatoriaPdf
    {
        byte[] Generar(CasaVelatoriaInformeDto informe);
    }
}
