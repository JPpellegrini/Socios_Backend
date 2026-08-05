using Socios.Application.DTOs;
using Socios.Application.UseCases.Entidades;

namespace Socios.Application.UseCases.Codeudores
{
    /// <summary>
    /// Casos de uso del codeudor.
    ///
    /// Hoy el alta de codeudor es, por debajo, un alta de ENTIDAD pura, así que este caso de uso
    /// solo delega en el de entidad y devuelve el Id de la entidad creada. La asignación del
    /// codeudor a un socio se resuelve en un trabajo aparte.
    /// </summary>
    public class CodeudorUseCase : ICodeudorUseCase
    {
        private readonly IEntidadUseCase _entidades;

        public CodeudorUseCase(IEntidadUseCase entidades)
        {
            _entidades = entidades;
        }

        /// <summary>Da de alta la entidad del codeudor y devuelve su Id de entidad.</summary>
        public Task<int> CrearAsync(CodeudorCrearDto dto)
        {
            // CodeudorCrearDto hereda de EntidadCrearDto: se pasa tal cual al alta de entidad.
            return _entidades.CrearAsync(dto);
        }
    }
}
