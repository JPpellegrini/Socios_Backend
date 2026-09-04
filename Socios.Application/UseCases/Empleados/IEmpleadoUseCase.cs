using Socios.Application.DTOs;

namespace Socios.Application.Interfaces
{
    public interface IEmpleadoUseCase
    {
        /// <summary>
        /// Alta de empleado. Devuelve el Id_Entidad generado.
        /// </summary>
        Task<int> CrearAsync(EmpleadoCrearDto dto);

        /// <summary>
        /// Visualizar un empleado por IdEntidad.
        /// </summary>
        Task<EmpleadoDto?> VisualizarAsync(int idEntidad);

        /// <summary>
        /// Dar de baja un empleado por IdEntidadTipo.
        /// </summary>
        Task<bool> BajaAsync(int idEntidadTipo);

        /// <summary>
        /// Modificar datos de un empleado existente.
        /// </summary>
        Task ModificarAsync(EmpleadoModificarDto dto);
        Task<List<EmpleadoDto>> BuscarAsync(string? busqueda);
    }
}