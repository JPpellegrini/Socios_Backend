using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface IEstadoCajaDiariaRepository
    {
        /// <summary>
        /// Trae la caja abierta del usuario: su último registro de caja diaria, si es una
        /// APERTURA (sin CIERRE posterior). Null si el usuario no tiene caja abierta.
        /// </summary>
        Task<EstadoCajaDiaria?> ObtenerCajaAbiertaAsync(int idUsuario);

        /// <summary>
        /// Trae el último CIERRE registrado por el usuario. Null si nunca cerró una caja.
        /// </summary>
        Task<EstadoCajaDiaria?> ObtenerUltimoCierreAsync(int idUsuario);

        /// <summary>
        /// Bloquea la numeración de cajas hasta el final de la transacción en curso, para que
        /// dos aperturas simultáneas no tomen el mismo número ni un usuario abra dos cajas con
        /// un doble clic. Debe llamarse dentro de una transacción explícita
        /// (IUnitOfWork.EjecutarEnTransaccionAsync).
        /// </summary>
        Task BloquearNumeracionAsync();

        /// <summary>Próximo número de caja: el mayor usado + 1 (1 si no hay ninguno).</summary>
        Task<int> ObtenerSiguienteNroCajaAsync();

        /// <summary>Marca el registro para ser insertado. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(EstadoCajaDiaria registro);
    }
}
