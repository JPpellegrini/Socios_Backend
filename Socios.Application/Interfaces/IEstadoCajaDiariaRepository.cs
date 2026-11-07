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
        /// Bloqueo EXCLUSIVO de las cajas hasta el final de la transacción en curso. Lo toman
        /// la apertura y el cierre: dos aperturas a la vez no toman el mismo número, un doble
        /// clic no abre ni cierra dos veces, y mientras se cierra no entra ningún cobro.
        /// Debe llamarse dentro de una transacción explícita (IUnitOfWork.EjecutarEnTransaccionAsync).
        /// </summary>
        Task BloquearCajasAsync();

        /// <summary>
        /// Bloqueo COMPARTIDO de las cajas hasta el final de la transacción en curso. Lo toman
        /// las operaciones que registran movimientos (cobros): entre ellas no se frenan, pero
        /// esperan a una apertura/cierre en curso y la hacen esperar. Así un movimiento nunca
        /// cae en una caja que se está cerrando. Debe llamarse dentro de una transacción explícita.
        /// </summary>
        Task BloquearCajasCompartidoAsync();

        /// <summary>Próximo número de caja: el mayor usado + 1 (1 si no hay ninguno).</summary>
        Task<int> ObtenerSiguienteNroCajaAsync();

        /// <summary>Marca el registro para ser insertado. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(EstadoCajaDiaria registro);
    }
}
