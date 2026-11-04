using Socios.Application.Interfaces;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure
{
    /// <summary>
    /// Implementación de la unidad de trabajo sobre Entity Framework.
    ///
    /// El DbContext de EF ya funciona como una unidad de trabajo: va acumulando los cambios
    /// que hacen los repositorios (todos comparten el MISMO DbContext) y los confirma todos
    /// juntos al llamar a SaveChanges. Acá simplemente le ponemos un nombre claro a esa idea.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SociosDbContext _context;

        public UnitOfWork(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<int> GuardarCambiosAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<T> EjecutarEnTransaccionAsync<T>(Func<Task<T>> operacion)
        {
            // Los SaveChanges de la operación se suman a esta transacción. Si la operación lanza
            // una excepción, al salir del using la transacción se descarta (rollback) sin confirmar.
            await using var transaccion = await _context.Database.BeginTransactionAsync();

            var resultado = await operacion();

            await transaccion.CommitAsync();
            return resultado;
        }
    }
}
