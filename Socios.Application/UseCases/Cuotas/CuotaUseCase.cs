using Socios.Application.DTOs;
using Socios.Application.Exceptions;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Application.UseCases.Cuotas
{
    /// <summary>
    /// Casos de uso de generación de cuotas. Coordina QUÉ hay que hacer (reglas de negocio)
    /// y delega el acceso a datos en los repositorios y la confirmación en la unidad de trabajo.
    /// </summary>
    public class CuotaUseCase : ICuotaUseCase
    {
        private readonly ISocioRepository _socios;
        private readonly ITipoCuotaRepository _tiposCuota;
        private readonly ITipoPlanRepository _tiposPlan;
        private readonly ICuotaRepository _cuotas;
        private readonly IUnitOfWork _unitOfWork;

        private const string ConceptoSocio = "SOCIO";
        private const string ConceptoSepelio = "SEPELIO";
        private const string SepelioSi = "SI";
        private const string EstadoPendiente = "PENDIENTE";

        public CuotaUseCase(
            ISocioRepository socios,
            ITipoCuotaRepository tiposCuota,
            ITipoPlanRepository tiposPlan,
            ICuotaRepository cuotas,
            IUnitOfWork unitOfWork)
        {
            _socios = socios;
            _tiposCuota = tiposCuota;
            _tiposPlan = tiposPlan;
            _cuotas = cuotas;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Genera las cuotas de un socio individual. Reglas:
        ///   - El socio tiene que existir.
        ///   - El vencimiento tiene que ser posterior a hoy.
        ///   - El período desde no puede ser posterior al período hasta.
        ///   - La cuota societaria (SOCIO) se genera en TODOS los meses del rango.
        ///   - La cuota de sepelio solo se genera si el socio tiene sepelio y para los meses
        ///     "hacia adelante" (período >= mes actual). El tramo (hasta/más de el tope) se
        ///     resuelve una sola vez, según la edad ACTUAL del socio y su plan.
        ///   - No se generan duplicados: si ya existe una cuota del mismo concepto para ese
        ///     mes/año, se omite.
        /// Todo se confirma en una sola transacción.
        /// </summary>
        public async Task<CuotaGeneracionResultadoDto> GenerarIndividualAsync(CuotaGenerarDto dto)
        {
            var hoy = DateTime.Today;

            // El socio tiene que existir; se trae con su entidad (nacimiento, id de entidad).
            var socio = await _socios.ObtenerConEntidadAsync(dto.IdSocio!.Value)
                ?? throw new RecursoNoEncontradoException("No se encontró el socio solicitado.");

            // Regla de negocio: el vencimiento tiene que ser a futuro.
            if (dto.FechaVencimiento!.Value.Date <= hoy)
                throw new ReglaNegocioException("La fecha de vencimiento debe ser posterior a hoy.");

            // Rango de períodos como primer día de cada mes, para iterar mes a mes.
            var desde = new DateTime(dto.AnioDesde!.Value, dto.MesDesde!.Value, 1);
            var hasta = new DateTime(dto.AnioHasta!.Value, dto.MesHasta!.Value, 1);
            var mesActual = new DateTime(hoy.Year, hoy.Month, 1);

            // Regla de negocio: el desde no puede ser posterior al hasta.
            if (desde > hasta)
                throw new ReglaNegocioException("El período desde no puede ser posterior al período hasta.");

            // Importe vigente de la cuota societaria (concepto SOCIO).
            var tipoSocio = await _tiposCuota.ObtenerSocioAsync()
                ?? throw new ReglaNegocioException("No está configurada la cuota del concepto SOCIO.");

            var generaSepelio = string.Equals(socio.Sepelio, SepelioSi, StringComparison.OrdinalIgnoreCase);
            var rangoIncluyeAdelante = hasta >= mesActual;

            // Si corresponde generar sepelio, se resuelve el tramo una sola vez (edad actual + plan).
            TipoCuota? tipoSepelio = null;
            if (generaSepelio && rangoIncluyeAdelante)
                tipoSepelio = await ResolverTipoCuotaSepelioAsync(socio, hoy);

            // Claves (concepto + mes + año) ya existentes en el rango, para no duplicar.
            var desdeClave = desde.Year * 100 + desde.Month;
            var hastaClave = hasta.Year * 100 + hasta.Month;
            var existentes = await _cuotas.ObtenerClavesPeriodoAsync(socio.Id_Entidad, desdeClave, hastaClave);
            var yaExiste = new HashSet<(string Concepto, int Mes, int Anio)>(
                existentes.Select(e => (e.Concepto, e.Mes, e.Anio)));

            var resultado = new CuotaGeneracionResultadoDto();

            for (var periodo = desde; periodo <= hasta; periodo = periodo.AddMonths(1))
            {
                // Cuota societaria: en todos los meses del rango (atrás y adelante).
                AgregarSiNoExiste(ConceptoSocio, tipoSocio.Id_TipoCuota, tipoSocio.Importe,
                    socio.Id_Entidad, periodo, dto.FechaVencimiento.Value, yaExiste, resultado);

                // Cuota de sepelio: solo si el socio tiene sepelio y el mes es hacia adelante.
                if (tipoSepelio != null && periodo >= mesActual)
                {
                    AgregarSiNoExiste(ConceptoSepelio, tipoSepelio.Id_TipoCuota, tipoSepelio.Importe,
                        socio.Id_Entidad, periodo, dto.FechaVencimiento.Value, yaExiste, resultado);
                }
            }

            // Si no se generó ninguna cuota (todo ya existía), no hay cambios que confirmar.
            if (resultado.CuotasGeneradas > 0)
                await _unitOfWork.GuardarCambiosAsync();

            return resultado;
        }

        /// <summary>
        /// Agrega una cuota si no existía ya (mismo concepto + mes + año). Actualiza el
        /// resumen (generadas/omitidas) y el set en memoria para no duplicar en el mismo lote.
        /// </summary>
        private void AgregarSiNoExiste(
            string concepto, int idTipoCuota, decimal monto, int idEntidad, DateTime periodo,
            DateTime vencimiento, HashSet<(string, int, int)> yaExiste, CuotaGeneracionResultadoDto resultado)
        {
            var clave = (concepto, periodo.Month, periodo.Year);
            if (!yaExiste.Add(clave))
            {
                resultado.CuotasOmitidas++;
                return;
            }

            _cuotas.Agregar(new Cuota
            {
                Id_Entidad = idEntidad,
                Mes_Periodo = periodo.Month,
                Anio_Periodo = periodo.Year,
                Id_TipoCuota = idTipoCuota,
                Monto = monto,
                FechaHoraGeneracion = DateTime.Now,
                FechaVencimiento = vencimiento,
                Estado = EstadoPendiente,
                Id_Movimiento = null
            });

            resultado.CuotasGeneradas++;
        }

        /// <summary>
        /// Resuelve el tipo de cuota de sepelio que le corresponde al socio, según su plan
        /// y su edad actual: tramo "hasta el tope" si edad &lt;= tope, "más de" si la supera.
        /// </summary>
        private async Task<TipoCuota> ResolverTipoCuotaSepelioAsync(Socio socio, DateTime hoy)
        {
            var plan = socio.Plan?.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(plan))
                throw new ReglaNegocioException("El socio tiene sepelio pero no tiene un plan asignado; no se puede determinar la cuota de sepelio.");

            if (socio.Entidad?.Nacimiento is null)
                throw new ReglaNegocioException("El socio tiene sepelio pero no tiene fecha de nacimiento; no se puede determinar el tramo de edad.");

            var planes = await _tiposPlan.ObtenerSepelioPorPlanAsync(plan);

            // El tramo "hasta" lleva el tope de edad; el tramo "más de" lo tiene en null.
            var tramoHasta = planes.FirstOrDefault(p => p.EdadTope.HasValue);
            var tramoMasDe = planes.FirstOrDefault(p => !p.EdadTope.HasValue);
            if (tramoHasta is null || tramoMasDe is null)
                throw new ReglaNegocioException($"El plan '{plan}' no tiene la configuración de sepelio completa (tramos hasta/más de).");

            var edad = CalcularEdad(socio.Entidad.Nacimiento.Value, hoy);

            // "hasta el tope" incluye la edad igual al tope; recién por encima pasa a "más de".
            var tramo = edad <= tramoHasta.EdadTope!.Value ? tramoHasta : tramoMasDe;
            return tramo.TipoCuota;
        }

        /// <summary>Edad en años cumplidos a la fecha indicada.</summary>
        private static int CalcularEdad(DateTime nacimiento, DateTime referencia)
        {
            var edad = referencia.Year - nacimiento.Year;
            if (nacimiento.Date > referencia.AddYears(-edad)) edad--;
            return edad;
        }
    }
}
