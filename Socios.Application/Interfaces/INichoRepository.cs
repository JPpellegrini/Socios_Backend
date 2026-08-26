using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    public interface INichoRepository
    {
        Task<List<NichoListadoDto>> BuscarNichoAsync(NichoFiltroDto filtro);

        /// <summary>Indica si ya existe un nicho con ese sector y número (comparación exacta).</summary>
        Task<bool> ExisteSectorNumeroAsync(string sector, string nroNicho);

        /// <summary>Trae el nicho por su Id (trackeado, para poder eliminarlo). Null si no existe.</summary>
        Task<Domain.Entities.Nicho?> ObtenerPorIdAsync(int idNicho);

        /// <summary>Marca un nicho para insertar. El guardado real lo dispara la unidad de trabajo.</summary>
        void Agregar(Domain.Entities.Nicho nicho);

        /// <summary>Marca un nicho para eliminar. El borrado real lo dispara la unidad de trabajo.</summary>
        void Eliminar(Domain.Entities.Nicho nicho);
    }
}
