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
        private readonly ITipoPlanRepository _tiposPlan;
        private readonly IUnitOfWork _unitOfWork;

        // Conceptos que este caso de uso maneja.
        private const string ConceptoSocio = "SOCIO";
        private const string ConceptoSepelio = "SEPELIO";

        public ConfiguracionCuotaUseCase(
            ITipoCuotaRepository tiposCuota,
            ITipoPlanRepository tiposPlan,
            IUnitOfWork unitOfWork)
        {
            _tiposCuota = tiposCuota;
            _tiposPlan = tiposPlan;
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

        /// <summary>
        /// Modifica la cuota de SEPELIO "hasta el tope". Reglas:
        ///   - El tipo de cuota tiene que existir.
        ///   - Tiene que ser del concepto SEPELIO.
        ///   - Tiene que ser la cuota "hasta el tope" (Tiene_EdadTope = true): la cuota
        ///     "más de" no tiene tope propio y se edita por su propio endpoint.
        /// Cambia el importe (en tipo_cuotas) y el tope de edad (en el plan asociado, en
        /// tipo_planes), y actualiza la fecha de última modificación. Todo en una sola
        /// transacción.
        ///
        /// El tope se guarda una sola vez acá: la cuota "más de" del mismo plan lo hereda,
        /// así que este cambio también corre la frontera que muestra el "MAS DE n".
        /// </summary>
        public async Task ModificarSepelioHastaAsync(SepelioHastaModificarDto dto)
        {
            // Trackeado, para poder modificarlo y que la unidad de trabajo lo confirme.
            var tipoCuota = await _tiposCuota.ObtenerPorIdAsync(dto.Id_TipoCuota!.Value)
                ?? throw new RecursoNoEncontradoException("No se encontró el tipo de cuota solicitado.");

            // Regla de negocio: este endpoint solo edita cuotas de SEPELIO...
            if (tipoCuota.Concepto != ConceptoSepelio)
                throw new ReglaNegocioException("Solo se puede modificar el importe y el tope de cuotas del concepto SEPELIO.");

            // ...y específicamente la cuota "hasta el tope" (la que tiene el límite de edad).
            if (!tipoCuota.Tiene_EdadTope)
                throw new ReglaNegocioException("Esta es la cuota 'más de' (sin tope propio): se modifica por el endpoint que solo cambia el importe.");

            // El tope vive en el plan asociado a esta cuota (relación 1 a 1).
            var tipoPlan = await _tiposPlan.ObtenerPorTipoCuotaAsync(tipoCuota.Id_TipoCuota)
                ?? throw new InvalidOperationException("La cuota de sepelio no tiene un plan asociado.");

            tipoCuota.Importe = dto.Importe!.Value;
            tipoCuota.Fecha_ultimamodif = DateTime.Now;
            tipoPlan.EdadTope = dto.EdadTope!.Value;

            await _unitOfWork.GuardarCambiosAsync();
        }

        /// <summary>
        /// Modifica la cuota de SEPELIO "más de" (sin tope propio). Reglas:
        ///   - El tipo de cuota tiene que existir.
        ///   - Tiene que ser del concepto SEPELIO.
        ///   - Tiene que ser la cuota "más de" (Tiene_EdadTope = false): la cuota "hasta el
        ///     tope" se edita por su propio endpoint (que además cambia la edad).
        /// Solo cambia el importe y actualiza la fecha de última modificación. La edad no se
        /// toca: va desde el tope de su cuota hermana hasta el infinito.
        /// </summary>
        public async Task ModificarSepelioMasDeAsync(SepelioMasDeModificarDto dto)
        {
            // Trackeado, para poder modificarlo y que la unidad de trabajo lo confirme.
            var tipoCuota = await _tiposCuota.ObtenerPorIdAsync(dto.Id_TipoCuota!.Value)
                ?? throw new RecursoNoEncontradoException("No se encontró el tipo de cuota solicitado.");

            // Regla de negocio: este endpoint solo edita cuotas de SEPELIO...
            if (tipoCuota.Concepto != ConceptoSepelio)
                throw new ReglaNegocioException("Solo se puede modificar el importe de cuotas del concepto SEPELIO.");

            // ...y específicamente la cuota "más de" (sin tope de edad propio).
            if (tipoCuota.Tiene_EdadTope)
                throw new ReglaNegocioException("Esta es la cuota 'hasta el tope': se modifica por el endpoint que también cambia la edad.");

            tipoCuota.Importe = dto.Importe!.Value;
            tipoCuota.Fecha_ultimamodif = DateTime.Now;

            await _unitOfWork.GuardarCambiosAsync();
        }
    }
}
