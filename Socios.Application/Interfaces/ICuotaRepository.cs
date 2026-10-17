using Socios.Application.DTOs;
using Socios.Domain.Entities;

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

        /// <summary>
        /// Trae las claves (concepto + mes + año) de las cuotas que ya tiene la entidad
        /// dentro del rango de períodos, para no generar duplicados. El rango se expresa
        /// como clave comparable año*100 + mes (ej. 202603).
        /// </summary>
        Task<List<CuotaClaveDto>> ObtenerClavesPeriodoAsync(int idEntidad, int desdeClave, int hastaClave);

        /// <summary>Marca la cuota para ser insertada. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Cuota cuota);
    }
}
