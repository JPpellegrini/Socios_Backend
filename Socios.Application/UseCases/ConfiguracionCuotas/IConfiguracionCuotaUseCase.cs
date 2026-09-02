using Socios.Application.DTOs;

namespace Socios.Application.UseCases.ConfiguracionCuotas
{
    /// <summary>
    /// Punto de entrada de los casos de uso de la configuración de cuotas.
    /// </summary>
    public interface IConfiguracionCuotaUseCase
    {
        /// <summary>
        /// Modifica el importe de la cuota del concepto SOCIO y actualiza la fecha de
        /// última modificación.
        /// </summary>
        Task ModificarImporteSocioAsync(ConfiguracionCuotaModificarDto dto);
    }
}
