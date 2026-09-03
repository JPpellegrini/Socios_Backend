using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<List<EntidadDto>> ListarEmpleadosAsync(EntidadFiltroBasicoDto filtro);
        Task<EntidadDto?> DarDeBajaEmpleadoAsync(int idEntidadTipo);
    }
}
