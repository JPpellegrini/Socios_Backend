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
        /// Trae las claves (concepto + mes + año) de las cuotas vigentes (no anuladas) que ya
        /// tiene la entidad dentro del rango de períodos, para no generar duplicados. El rango
        /// se expresa como clave comparable año*100 + mes (ej. 202603).
        /// </summary>
        Task<List<CuotaClaveDto>> ObtenerClavesPeriodoAsync(int idEntidad, int desdeClave, int hastaClave);

        /// <summary>
        /// Trae las claves (entidad + concepto) de todas las cuotas vigentes (no anuladas) de
        /// un mes/año, de todos los socios, para no duplicar en la generación masiva.
        /// </summary>
        Task<List<CuotaClaveDto>> ObtenerClavesMesAsync(int mes, int anio);

        /// <summary>Marca la cuota para ser insertada. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Cuota cuota);

        /// <summary>
        /// Trae la cuota por su Id (Id_Deuda), con su tipo de cuota incluido (para conocer
        /// el concepto) y trackeada para poder modificarla (anular). Null si no existe.
        /// </summary>
        Task<Cuota?> ObtenerPorIdAsync(int idDeuda);

        /// <summary>
        /// Trae el lote de cuotas de NICHO de una entidad generadas en el mismo instante
        /// (misma FechaHoraGeneracion), trackeadas, para anularlas en cascada. El instante
        /// compartido identifica las cuotas creadas en una misma asignación de nicho.
        /// </summary>
        Task<List<Cuota>> ObtenerLoteNichoAsync(int idEntidad, DateTime fechaHoraGeneracion);
    }
}
