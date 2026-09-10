using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    public interface IColaboradorUseCase
    {
        Task<List<ColaboradorDto>> BuscarAsync(string? busqueda);

        /// <summary>
        /// Dar de baja un colabordor por IdEntidadTipo.
        /// </summary>
        Task<bool> BajaAsync(int idEntidadTipo, string motivo);

        /// <summary>Da de alta un colaborador y devuelve el Id generado.</summary>
        Task<int> CrearAsync(ColaboradorCrearDto dto);

        /// <summary>Modifica los datos editables de un proveedor (razón social, servicio, domicilio y contactos).</summary>
        Task ModificarAsync(ColaboradorModificarDto dto);

        /// <summary>Reactiva un colaborador dado de baja (pasa su estado a ACTIVO).</summary>
        Task ReactivarAsync(ColaboradorReactivarDto dto);

    }

}