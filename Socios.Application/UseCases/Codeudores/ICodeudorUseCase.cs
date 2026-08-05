using Socios.Application.DTOs;

namespace Socios.Application.UseCases.Codeudores
{
    public interface ICodeudorUseCase
    {
        Task<int> CrearAsync(CodeudorCrearDto dto);
    }
}
