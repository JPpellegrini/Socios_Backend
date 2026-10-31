namespace Socios.Application.Interfaces
{
    public interface ITipoPlanRepository
    {
        /// <summary>
        /// Trae el plan asociado a un tipo de cuota (trackeado, para poder modificar su
        /// edad tope). Null si ese tipo de cuota no tiene plan.
        /// </summary>
        Task<Domain.Entities.TipoPlan?> ObtenerPorTipoCuotaAsync(int idTipoCuota);

        /// <summary>
        /// Trae todos los planes de SEPELIO (A y B, tramos "hasta" y "más de") con su tipo de
        /// cuota incluido. Por cada plan hay dos filas: la del tramo "hasta" (EdadTope con
        /// valor) y la del tramo "más de" (EdadTope null). Se usa para elegir el tramo según
        /// plan y edad; se trae una sola vez aunque se generen cuotas para muchos socios.
        /// </summary>
        Task<List<Domain.Entities.TipoPlan>> ObtenerSepelioAsync();
    }
}
