using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Arma el PDF del estado de cuenta individual del socio.
    /// La implementación (librería de PDF, diseño de la hoja) vive en Infraestructura.
    /// </summary>
    public interface IEstadoCuentaSocioPdf
    {
        byte[] Generar(SocioEstadoCuentaDto estadoCuenta);
    }
}
