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
        private readonly ISocioRepository _socios;
        private readonly IEntidadTipoRepository _entidadesTipo;
        private readonly IUnitOfWork _unitOfWork;

        // Id del tipo de entidad "Socio" (catálogo TiposEntidad).
        private const int IdTipoSocio = 1;

        public NichoUseCase(
            INichoRepository nichos,
            ISocioRepository socios,
            IEntidadTipoRepository entidadesTipo,
            IUnitOfWork unitOfWork)
        {
            _nichos = nichos;
            _socios = socios;
            _entidadesTipo = entidadesTipo;
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

        /// <summary>
        /// Da de baja un nicho (borrado físico). Reglas:
        ///   - El nicho tiene que existir.
        ///   - No se puede eliminar un nicho ocupado (Ocupado = "SI").
        /// </summary>
        public async Task BajaAsync(NichoBajaDto dto)
        {
            var nicho = await _nichos.ObtenerPorIdAsync(dto.IdNicho)
                ?? throw new RecursoNoEncontradoException("No se encontró el nicho solicitado.");

            // Regla de negocio: un nicho ocupado no se elimina.
            if (nicho.Ocupado == "SI")
                throw new ReglaNegocioException("No se puede eliminar un nicho que está ocupado.");

            _nichos.Eliminar(nicho);

            await _unitOfWork.GuardarCambiosAsync();
        }

        /// <summary>
        /// Asigna un nicho libre a un socio. Reglas:
        ///   - El nicho tiene que existir.
        ///   - El nicho no puede estar ya ocupado.
        ///   - La entidad indicada tiene que ser un socio.
        ///   - Si lleva lápida, el valor de la lápida es obligatorio.
        /// Carga valor, cuotas, interés y lápida, y marca el nicho como ocupado.
        /// </summary>
        public async Task AsignarAsync(NichoAsignarDto dto)
        {
            var nicho = await _nichos.ObtenerPorIdAsync(dto.IdNicho!.Value)
                ?? throw new RecursoNoEncontradoException("No se encontró el nicho solicitado.");

            // Regla de negocio: un nicho ocupado no se puede volver a asignar.
            if (nicho.Ocupado == "SI")
                throw new ReglaNegocioException("El nicho ya está ocupado y no se puede asignar.");

            // La entidad que va a ocupar el nicho tiene que ser un socio.
            if (!await _socios.EsSocioAsync(dto.IdEntidad!.Value))
                throw new ReglaNegocioException("La entidad indicada no está registrada como socio.");

            // Y ese socio tiene que estar activo (el estado vive en EntidadTipo, tipo Socio).
            var socioTipo = await _entidadesTipo.ObtenerPorEntidadYTipoAsync(dto.IdEntidad.Value, IdTipoSocio);
            if (socioTipo is null || socioTipo.Estado != "ACTIVO")
                throw new ReglaNegocioException("El socio no está activo: no se le puede asignar un nicho.");

            // Normalizamos "si"/"no" (cualquier capitalización) al "SI"/"NO" que se persiste.
            var llevaLapida = dto.ConLapida.Trim().Equals("SI", StringComparison.OrdinalIgnoreCase);

            // Si lleva lápida, el valor de la lápida es obligatorio; si no, se ignora.
            if (llevaLapida && dto.ValorLapida is null)
                throw new ReglaNegocioException("Debe informar el valor de la lápida.");

            nicho.Id_Entidad = dto.IdEntidad;
            nicho.ValorNicho = dto.ValorTotal;
            nicho.Cuotas = dto.Cuotas;
            nicho.InteresMensual = dto.InteresPorCuota;
            nicho.ConLapida = llevaLapida ? "SI" : "NO";
            nicho.ValorLapida = llevaLapida ? dto.ValorLapida : null;
            nicho.Ocupado = "SI";

            await _unitOfWork.GuardarCambiosAsync();
        }
    }
}
