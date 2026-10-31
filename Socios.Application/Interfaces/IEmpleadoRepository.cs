using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<Entidad?> ObtenerPorDniAsync(string dni);
        Task<bool> EsEmpleadoAsync(int idEntidad);
        Task<Entidad?> ObtenerPorIdAsync(int idEntidad);
        Task<EntidadTipo?> ObtenerEntidadTipoAsync(int idEntidadTipo);

        void AgregarEntidad(Entidad entidad);
        void AgregarEntidadTipo(EntidadTipo entidadTipo);
        void ActualizarEntidad(Entidad entidad);
        void ActualizarEntidadTipo(EntidadTipo entidadTipo);
        Task<List<Entidad>> BuscarAsync(string? busqueda);
        Task RegistrarBajaAsync(EntidadBaja baja);
    }
}