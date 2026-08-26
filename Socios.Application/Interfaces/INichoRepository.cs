using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    public interface INichoRepository
    {
        Task<List<NichoListadoDto>> BuscarNichoAsync(NichoFiltroDto filtro);

        /// <summary>Indica si ya existe un nicho con ese sector y número (comparación exacta).</summary>
        Task<bool> ExisteSectorNumeroAsync(string sector, string nroNicho);

        /// <summary>Marca un nicho para insertar. El guardado real lo dispara la unidad de trabajo.</summary>
        void Agregar(Domain.Entities.Nicho nicho);
    }
}
