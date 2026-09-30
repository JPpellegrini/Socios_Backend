using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    public interface ICuotaRepository
    {
        /// <summary>
        /// Trae las cuotas para la grilla, cruzadas con el socio y el tipo de cuota.
        /// Filtros opcionales (combinados con AND): socio (Nombre/Apellido/DNI),
        /// tipo de cuota (concepto) y estado.
        /// </summary>
        Task<List<CuotaListadoDto>> BuscarAsync(CuotaFiltroDto filtro);
    }
}
