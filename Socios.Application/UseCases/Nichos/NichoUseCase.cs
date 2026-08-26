using Socios.Application.DTOs;
using Socios.Application.Exceptions;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Application.UseCases.Nichos
{
    /// <summary>
    /// Reúne los casos de uso del nicho. Coordina QUÉ hay que hacer y delega el acceso a
    /// datos en el repositorio y la confirmación en la unidad de trabajo.
    /// </summary>
    public class NichoUseCase : INichoUseCase
    {
        private readonly INichoRepository _nichos;
        private readonly IUnitOfWork _unitOfWork;

        public NichoUseCase(INichoRepository nichos, IUnitOfWork unitOfWork)
        {
            _nichos = nichos;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Alta de nicho. Solo se informan sector y número. Reglas:
        ///   - No puede existir otro nicho con el mismo sector y número.
        /// El nicho nace libre (Ocupado = "NO") y sin lápida (ConLapida = "NO"); el valor y
        /// los datos de financiación se completan recién al asignarlo a un socio.
        /// </summary>
        public async Task<int> CrearAsync(NichoCrearDto dto)
        {
            var sector = dto.Sector.Trim();
            var nroNicho = dto.NroNicho.Trim();

            // Regla de negocio: sector + número identifican al nicho, no se pueden repetir.
            if (await _nichos.ExisteSectorNumeroAsync(sector, nroNicho))
                throw new ReglaNegocioException($"Ya existe un nicho en el sector '{sector}' con el número '{nroNicho}'.");

            var nicho = new Nicho
            {
                Sector = sector,
                NroNicho = nroNicho,
                Ocupado = "NO",
                ConLapida = "NO"
            };

            _nichos.Agregar(nicho);

            await _unitOfWork.GuardarCambiosAsync();

            // El id se completa después de guardar.
            return nicho.Id_Nicho;
        }
    }
}
