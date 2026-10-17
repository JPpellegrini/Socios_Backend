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
        /// Trae los planes de SEPELIO de un plan (A/B) con su tipo de cuota incluido.
        /// Devuelve las dos filas del plan: la del tramo "hasta" (EdadTope con valor) y la
        /// del tramo "más de" (EdadTope null). Se usa para elegir el tramo según la edad.
        /// </summary>
        Task<List<Domain.Entities.TipoPlan>> ObtenerSepelioPorPlanAsync(string plan);
    }
}
