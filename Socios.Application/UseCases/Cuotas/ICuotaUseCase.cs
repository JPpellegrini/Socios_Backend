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
        /// si el socio tiene sepelio. No genera duplicados. Un error de carga del socio no
        /// corta la generación: se informa en el resultado.
        /// </summary>
        Task<CuotaGeneracionResultadoDto> GenerarIndividualAsync(CuotaGenerarDto dto);

        /// <summary>
        /// Genera las cuotas de un mes/año para todos los socios activos (societaria y, si
        /// corresponde, sepelio). No genera duplicados. Los socios con errores de carga no
        /// cortan el proceso: se informan en el resultado.
        /// </summary>
        Task<CuotaGeneracionMasivaResultadoDto> GenerarMasivaAsync(CuotaGenerarMasivaDto dto);

        /// <summary>
        /// Anula (lógicamente) una cuota: pasa su estado a ANULADA. Solo se permite dentro
        /// de la hora de generada y si la cuota no está paga.
        /// </summary>
        Task AnularAsync(CuotaAnularDto dto);
    }
}
