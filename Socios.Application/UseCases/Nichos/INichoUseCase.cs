using Socios.Application.DTOs;

namespace Socios.Application.UseCases.Nichos
{
    /// <summary>
    /// Punto de entrada de los casos de uso del nicho.
    /// Define QUÉ operaciones existen, sin atarse a CÓMO se resuelven por dentro.
    /// </summary>
    public interface INichoUseCase
    {
        /// <summary>Da de alta un nicho (solo sector y número) y devuelve el Id generado.</summary>
        Task<int> CrearAsync(NichoCrearDto dto);

        /// <summary>Da de baja un nicho (borrado físico). Solo se puede si el nicho no está ocupado.</summary>
        Task BajaAsync(NichoBajaDto dto);
    }
}
