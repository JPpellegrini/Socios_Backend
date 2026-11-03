using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface IEstadoCajaDiariaRepository
    {
        /// <summary>
        /// Trae la caja abierta del usuario: su último registro de caja diaria, si es una
        /// APERTURA (sin CIERRE posterior). Null si el usuario no tiene caja abierta.
        /// </summary>
        Task<EstadoCajaDiaria?> ObtenerCajaAbiertaAsync(int idUsuario);
    }
}
