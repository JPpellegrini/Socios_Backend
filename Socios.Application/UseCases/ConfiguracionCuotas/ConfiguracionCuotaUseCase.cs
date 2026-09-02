using Socios.Application.DTOs;
using Socios.Application.Exceptions;
using Socios.Application.Interfaces;

namespace Socios.Application.UseCases.ConfiguracionCuotas
{
    /// <summary>
    /// Casos de uso de la configuración de cuotas. Coordina QUÉ hay que hacer y delega el
    /// acceso a datos en el repositorio y la confirmación en la unidad de trabajo.
    /// </summary>
    public class ConfiguracionCuotaUseCase : IConfiguracionCuotaUseCase
    {
        private readonly ITipoCuotaRepository _tiposCuota;
        private readonly IUnitOfWork _unitOfWork;

        // Único concepto que este caso de uso tiene permitido modificar.
        private const string ConceptoSocio = "SOCIO";

        public ConfiguracionCuotaUseCase(ITipoCuotaRepository tiposCuota, IUnitOfWork unitOfWork)
        {
            _tiposCuota = tiposCuota;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Modifica el importe de la cuota del concepto SOCIO. Reglas:
        ///   - El tipo de cuota tiene que existir.
        ///   - Tiene que ser del concepto SOCIO (no se permite editar SEPELIO/NICHO acá,
        ///     porque esos dependen de planes y edad tope).
        /// Además, actualiza la fecha de última modificación.
        /// </summary>
        public async Task ModificarImporteSocioAsync(ConfiguracionCuotaModificarDto dto)
        {
            // Trackeado, para poder modificarlo y que la unidad de trabajo lo confirme.
            var tipoCuota = await _tiposCuota.ObtenerPorIdAsync(dto.Id_TipoCuota!.Value)
                ?? throw new RecursoNoEncontradoException("No se encontró el tipo de cuota solicitado.");

            // Regla de negocio: este endpoint solo edita la cuota de SOCIO.
            if (tipoCuota.Concepto != ConceptoSocio)
                throw new ReglaNegocioException("Solo se puede modificar el importe de la cuota del concepto SOCIO.");

            tipoCuota.Importe = dto.Importe!.Value;
            tipoCuota.Fecha_ultimamodif = DateTime.Now;

            await _unitOfWork.GuardarCambiosAsync();
        }
    }
}
