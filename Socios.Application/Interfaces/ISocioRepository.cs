using Socios.Application.DTOs;
using Socios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socios.Application.Interfaces
{
    public interface ISocioRepository
    {
        Task<List<SocioListadoDto>> BuscarAsync(SocioFiltroDto filtro);

        /// <summary>Trae todos los datos de un socio para visualizarlo, o null si no existe.</summary>
        Task<SocioDetalleDto?> ObtenerDetalleAsync(int idSocio);
        Task<int?> ObtenerIdEntidadAsync(int idSocio);

        /// <summary>
        /// Trae el socio (con su entidad asociada) trackeado por EF, para poder modificarlo.
        /// Devuelve null si no existe.
        /// </summary>
        Task<Socio?> ObtenerConEntidadAsync(int idSocio);

        /// <summary>Indica si la entidad indicada ya está registrada como socio.</summary>
        Task<bool> EsSocioAsync(int idEntidad);

        /// <summary>Marca el socio para ser insertado. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Socio socio);
    }
}
