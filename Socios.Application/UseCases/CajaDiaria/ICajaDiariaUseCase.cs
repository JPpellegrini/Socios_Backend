using Socios.Application.DTOs;

namespace Socios.Application.UseCases.CajaDiaria
{
    /// <summary>
    /// Punto de entrada de los casos de uso de la caja diaria (apertura y cierre).
    /// </summary>
    public interface ICajaDiariaUseCase
    {
        /// <summary>
        /// Abre una caja nueva para el usuario con el efectivo declarado. Falla si el usuario
        /// ya tiene una caja abierta.
        /// </summary>
        Task<CajaAperturaResultadoDto> AbrirAsync(CajaAperturaDto dto, int idUsuario);
    }
}
