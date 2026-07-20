namespace Socios.Application.Interfaces
{
    /// <summary>
    /// Representa una "unidad de trabajo".
    ///
    /// La idea es simple: los repositorios solo van marcando cambios (agregar, modificar),
    /// pero NO los guardan por su cuenta. Es esta unidad de trabajo la que, cuando el caso
    /// de uso lo decide, confirma TODOS los cambios pendientes de una sola vez.
    ///
    /// ¿Por qué? Para que todo sea "todo o nada": si algo falla en el medio, no queda
    /// ninguna tabla grabada a medias (una única transacción).
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Confirma en la base de datos todos los cambios pendientes, en una sola transacción.
        /// Devuelve la cantidad de filas afectadas.
        /// </summary>
        Task<int> GuardarCambiosAsync();
    }
}
