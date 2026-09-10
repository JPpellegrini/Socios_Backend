using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    public interface IColaboradorUseCase
    {
        Task<List<ColaboradorDto>> BuscarAsync(string? busqueda);

        /// <summary>
        /// Dar de baja un colabordor por IdEntidadTipo.
        /// </summary>
        Task<bool> BajaAsync(int idEntidadTipo, string motivo);

    }

}