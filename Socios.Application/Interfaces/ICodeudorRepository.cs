using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface ICodeudorRepository
    {
        /// <summary>Marca la relación socio↔codeudor para ser insertada. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Codeudor codeudor);
    }
}
