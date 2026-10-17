using Socios.Application.DTOs;

namespace Socios.Application.UseCases.Cuotas
{
    /// <summary>
    /// Punto de entrada de los casos de uso de generación de cuotas.
    /// </summary>
    public interface ICuotaUseCase
    {
        /// <summary>
        /// Genera las cuotas de un socio individual en un rango de períodos (mes/año).
        /// La societaria se genera en todo el rango; la de sepelio solo hacia adelante y
        /// si el socio tiene sepelio. No genera duplicados.
        /// </summary>
        Task<CuotaGeneracionResultadoDto> GenerarIndividualAsync(CuotaGenerarDto dto);
    }
}
