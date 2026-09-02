namespace Socios.Application.Interfaces
{
    public interface ITipoPlanRepository
    {
        /// <summary>
        /// Trae el plan asociado a un tipo de cuota (trackeado, para poder modificar su
        /// edad tope). Null si ese tipo de cuota no tiene plan.
        /// </summary>
        Task<Domain.Entities.TipoPlan?> ObtenerPorTipoCuotaAsync(int idTipoCuota);
    }
}
