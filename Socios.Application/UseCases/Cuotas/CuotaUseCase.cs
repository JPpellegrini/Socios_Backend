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
        private readonly IEntidadTipoRepository _entidadesTipo;
        private readonly ITipoCuotaRepository _tiposCuota;
        private readonly ITipoPlanRepository _tiposPlan;
        private readonly ICuotaRepository _cuotas;
        private readonly IUnitOfWork _unitOfWork;

        private const string ConceptoSocio = "SOCIO";
        private const string ConceptoSepelio = "SEPELIO";
        private const string ConceptoNicho = "NICHO";
        private const string SepelioSi = "SI";
        private const string EstadoPendiente = "PENDIENTE";
        private const string EstadoPagado = "PAGADO";
        private const string EstadoAnulada = "ANULADA";

        // Ventana para poder anular una cuota desde que se generó.
        private static readonly TimeSpan VentanaAnulacion = TimeSpan.FromHours(1);

        // Estado y tipo del socio en EntidadTipo (donde vive el ACTIVO/INACTIVO).
        private const int IdTipoSocio = 1;
        private const string EstadoActivo = "ACTIVO";

        public CuotaUseCase(
            ISocioRepository socios,
            IEntidadTipoRepository entidadesTipo,
            ITipoCuotaRepository tiposCuota,
            ITipoPlanRepository tiposPlan,
            ICuotaRepository cuotas,
            IUnitOfWork unitOfWork)
        {
            _socios = socios;
            _entidadesTipo = entidadesTipo;
            _tiposCuota = tiposCuota;
            _tiposPlan = tiposPlan;
            _cuotas = cuotas;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Genera las cuotas de un socio individual. Reglas:
        ///   - El socio tiene que existir y estar activo.
        ///   - El vencimiento tiene que ser posterior a hoy.
        ///   - El período desde no puede ser posterior al período hasta.
        ///   - La cuota societaria (SOCIO) se genera en TODOS los meses del rango.
        ///   - La cuota de sepelio solo se genera si el socio tiene sepelio y para los meses
        ///     "hacia adelante" (período >= mes actual). El tramo (hasta/más de el tope) se
        ///     resuelve una sola vez, según la edad ACTUAL del socio y su plan.
        ///   - No se generan duplicados: si ya existe una cuota vigente del mismo concepto
        ///     para ese mes/año, se omite.
        /// Un error de carga del socio (sepelio sin plan, sin nacimiento o con un plan sin
        /// configuración) hace que NO se genere ninguna cuota (ni la societaria); no es un
        /// error HTTP: se devuelve el motivo en el resultado, igual que en la masiva.
        /// Todo se confirma en una sola transacción.
        /// </summary>
        public async Task<CuotaGeneracionResultadoDto> GenerarIndividualAsync(CuotaGenerarDto dto)
        {
            var hoy = DateTime.Today;

            // El socio tiene que existir; se trae con su entidad (nacimiento, id de entidad).
            var socio = await _socios.ObtenerConEntidadAsync(dto.IdSocio!.Value)
                ?? throw new RecursoNoEncontradoException("No se encontró el socio solicitado.");

            // Regla de negocio: a un socio inactivo no se le generan cuotas. El estado vive
            // en EntidadTipo (fila tipo Socio), no en Socio/Entidad.
            var socioTipo = await _entidadesTipo.ObtenerPorEntidadYTipoAsync(socio.Id_Entidad, IdTipoSocio);
            if (socioTipo is null || socioTipo.Estado != EstadoActivo)
                throw new ReglaNegocioException("El socio no está activo: no se le pueden generar cuotas.");

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

            var resultado = new CuotaGeneracionResultadoDto();

            // Si corresponde generar sepelio, se resuelve el tramo una sola vez (edad actual + plan).
            // Con un error de carga del socio no se genera NINGUNA cuota (ni la societaria):
            // se informa el motivo para que lo corrijan y vuelvan a generar.
            TipoCuota? tipoSepelio = null;
            if (generaSepelio && rangoIncluyeAdelante)
            {
                var planesSepelio = await _tiposPlan.ObtenerSepelioAsync();
                tipoSepelio = ResolverTramoSepelio(socio.Plan, socio.Entidad?.Nacimiento, planesSepelio, hoy, out var motivo);
                if (tipoSepelio is null)
                {
                    resultado.Inconsistencias.Add(new SocioInconsistenciaDto { IdSocio = socio.Id_Socio, Motivo = motivo! });
                    return resultado;
                }
            }

            // Claves (concepto + mes + año) ya existentes en el rango, para no duplicar.
            var desdeClave = desde.Year * 100 + desde.Month;
            var hastaClave = hasta.Year * 100 + hasta.Month;
            var existentes = await _cuotas.ObtenerClavesPeriodoAsync(socio.Id_Entidad, desdeClave, hastaClave);
            var yaExiste = new HashSet<(string Concepto, int Mes, int Anio)>(
                existentes.Select(e => (e.Concepto, e.Mes, e.Anio)));

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
        /// Genera las cuotas de UN mes/año para todos los socios activos. Mismas reglas que
        /// la individual, aplicadas a cada socio:
        ///   - El vencimiento tiene que ser posterior a hoy.
        ///   - A cada socio activo se le genera la cuota societaria (SOCIO).
        ///   - Si el socio tiene sepelio y el mes es "hacia adelante" (>= mes actual), también
        ///     la de sepelio, con el tramo según su plan y su edad ACTUAL. Para un mes pasado
        ///     solo se genera la societaria.
        ///   - No se generan duplicados: si el socio ya tiene una cuota vigente del mismo
        ///     concepto para ese mes, se omite.
        /// Un socio con un error de carga (sepelio sin plan, sin nacimiento o con un plan sin
        /// configuración) no recibe NINGUNA cuota (ni la societaria) y se lo informa en el
        /// resultado; el proceso sigue con los demás socios. El error se informa siempre,
        /// aunque sus cuotas del mes ya existan.
        /// Los datos se leen en pocas consultas (no una por socio) y todo se confirma en una
        /// sola transacción.
        /// </summary>
        public async Task<CuotaGeneracionMasivaResultadoDto> GenerarMasivaAsync(CuotaGenerarMasivaDto dto)
        {
            var hoy = DateTime.Today;

            // Regla de negocio: el vencimiento tiene que ser a futuro.
            if (dto.FechaVencimiento!.Value.Date <= hoy)
                throw new ReglaNegocioException("La fecha de vencimiento debe ser posterior a hoy.");

            var periodo = new DateTime(dto.Anio!.Value, dto.Mes!.Value, 1);
            var mesActual = new DateTime(hoy.Year, hoy.Month, 1);
            var esHaciaAdelante = periodo >= mesActual;

            // Importe vigente de la cuota societaria (concepto SOCIO).
            var tipoSocio = await _tiposCuota.ObtenerSocioAsync()
                ?? throw new ReglaNegocioException("No está configurada la cuota del concepto SOCIO.");

            var socios = await _socios.ObtenerActivosParaCuotasAsync();

            var resultado = new CuotaGeneracionMasivaResultadoDto { SociosProcesados = socios.Count };
            if (socios.Count == 0)
                return resultado;

            // Planes de sepelio: solo hacen falta si el mes es hacia adelante.
            IReadOnlyCollection<TipoPlan> planesSepelio = esHaciaAdelante
                ? await _tiposPlan.ObtenerSepelioAsync()
                : Array.Empty<TipoPlan>();

            // Cuotas vigentes del mes, de todos los socios, en una sola consulta.
            var existentes = await _cuotas.ObtenerClavesMesAsync(periodo.Month, periodo.Year);
            var yaExiste = new HashSet<(int IdEntidad, string Concepto)>(
                existentes.Select(e => (e.IdEntidad, e.Concepto)));

            var vencimiento = dto.FechaVencimiento.Value;
            var fechaGeneracion = DateTime.Now;

            foreach (var socio in socios)
            {
                // Si al socio le corresponde sepelio (lo tiene y el mes es hacia adelante), primero
                // se validan sus datos. Con un error de carga no se le genera NINGUNA cuota (ni la
                // societaria): se informa el motivo y se sigue con el próximo socio. El error se
                // informa siempre, aunque sus cuotas del mes ya existan (igual que en la individual).
                TipoCuota? tipoSepelio = null;
                var tieneSepelio = string.Equals(socio.Sepelio, SepelioSi, StringComparison.OrdinalIgnoreCase);
                if (esHaciaAdelante && tieneSepelio)
                {
                    tipoSepelio = ResolverTramoSepelio(socio.Plan, socio.Nacimiento, planesSepelio, hoy, out var motivo);
                    if (tipoSepelio is null)
                    {
                        resultado.Inconsistencias.Add(new SocioInconsistenciaDto { IdSocio = socio.IdSocio, Motivo = motivo! });
                        continue;
                    }
                }

                // Cuota societaria: a todos los socios activos sin errores de carga.
                if (yaExiste.Add((socio.IdEntidad, ConceptoSocio)))
                {
                    _cuotas.Agregar(NuevaCuota(socio.IdEntidad, tipoSocio.Id_TipoCuota, tipoSocio.Importe,
                        periodo, vencimiento, fechaGeneracion));
                    resultado.CuotasGeneradas++;
                }
                else
                {
                    resultado.CuotasOmitidas++;
                }

                // Cuota de sepelio: solo si le corresponde (tramo ya resuelto arriba).
                if (tipoSepelio is null)
                    continue;

                if (yaExiste.Add((socio.IdEntidad, ConceptoSepelio)))
                {
                    _cuotas.Agregar(NuevaCuota(socio.IdEntidad, tipoSepelio.Id_TipoCuota, tipoSepelio.Importe,
                        periodo, vencimiento, fechaGeneracion));
                    resultado.CuotasGeneradas++;
                }
                else
                {
                    resultado.CuotasOmitidas++;
                }
            }

            // Si no se generó ninguna cuota (todo ya existía), no hay cambios que confirmar.
            if (resultado.CuotasGeneradas > 0)
                await _unitOfWork.GuardarCambiosAsync();

            return resultado;
        }

        /// <summary>
        /// Anula (lógicamente) una cuota. Reglas:
        ///   - La cuota tiene que existir.
        ///   - No se puede anular una cuota paga (estado PAGADO).
        ///   - No se puede volver a anular una cuota ya anulada.
        ///   - Solo se puede anular dentro de la hora de generada.
        /// La anulación pasa el estado a ANULADA (no borra la fila).
        /// </summary>
        public async Task AnularAsync(CuotaAnularDto dto)
        {
            // Trackeada (con su tipo de cuota) para poder modificarla y para conocer el concepto.
            var cuota = await _cuotas.ObtenerPorIdAsync(dto.IdCuota!.Value)
                ?? throw new RecursoNoEncontradoException("No se encontró la cuota solicitada.");

            // Las cuotas de nicho se anulan en cascada: todo el lote generado junto.
            if (cuota.TipoCuota?.Concepto == ConceptoNicho)
            {
                await AnularLoteNichoAsync(cuota);
                return;
            }

            // Regla de negocio: una cuota paga no se anula.
            if (cuota.Estado == EstadoPagado)
                throw new ReglaNegocioException("No se puede anular una cuota que ya está paga.");

            // Regla de negocio: no tiene sentido anular dos veces.
            if (cuota.Estado == EstadoAnulada)
                throw new ReglaNegocioException("La cuota ya está anulada.");

            // Regla de negocio: solo dentro de la hora desde que se generó.
            if (DateTime.Now - cuota.FechaHoraGeneracion > VentanaAnulacion)
                throw new ReglaNegocioException("Solo se puede anular una cuota dentro de la hora de generada.");

            cuota.Estado = EstadoAnulada;

            await _unitOfWork.GuardarCambiosAsync();
        }

        /// <summary>
        /// Anula en cascada el lote de cuotas de nicho generado junto con la cuota indicada
        /// (mismo instante de generación). Reglas:
        ///   - Solo dentro de la hora de generado (todas comparten FechaHoraGeneracion).
        ///   - Si alguna cuota del lote ya está paga, no se anula ninguna.
        ///   - Si ya estaban todas anuladas, no hay nada que anular.
        /// Pasa todas las cuotas del lote a ANULADA en una sola transacción.
        /// </summary>
        private async Task AnularLoteNichoAsync(Cuota cuota)
        {
            var lote = await _cuotas.ObtenerLoteNichoAsync(cuota.Id_Entidad, cuota.FechaHoraGeneracion);

            // Ventana: todas las del lote comparten el instante de generación; una verificación basta.
            if (DateTime.Now - cuota.FechaHoraGeneracion > VentanaAnulacion)
                throw new ReglaNegocioException("Solo se pueden anular las cuotas de nicho dentro de la hora de generadas.");

            // Si cualquiera del lote ya está paga, no se anula ninguna.
            if (lote.Any(c => c.Estado == EstadoPagado))
                throw new ReglaNegocioException("No se pueden anular las cuotas del nicho: alguna ya está paga.");

            // Si ya estaban todas anuladas, no hay nada que hacer.
            if (lote.All(c => c.Estado == EstadoAnulada))
                throw new ReglaNegocioException("Las cuotas del nicho ya están anuladas.");

            foreach (var c in lote.Where(c => c.Estado != EstadoAnulada))
                c.Estado = EstadoAnulada;

            await _unitOfWork.GuardarCambiosAsync();
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

            _cuotas.Agregar(NuevaCuota(idEntidad, idTipoCuota, monto, periodo, vencimiento, DateTime.Now));

            resultado.CuotasGeneradas++;
        }

        /// <summary>
        /// Resuelve el tipo de cuota de sepelio que le corresponde a un socio, según su plan
        /// y su edad actual: tramo "hasta el tope" si edad &lt;= tope, "más de" si la supera.
        /// Es una función pura (no consulta la base): recibe todos los planes de sepelio ya
        /// cargados, así sirve igual para un socio que para miles. Si el dato del socio no
        /// alcanza para decidir, devuelve null y el motivo; quien llama decide si corta
        /// (individual) o lo informa y sigue (masiva).
        /// </summary>
        private static TipoCuota? ResolverTramoSepelio(
            string? planSocio, DateTime? nacimiento, IReadOnlyCollection<TipoPlan> planesSepelio,
            DateTime hoy, out string? motivo)
        {
            var plan = planSocio?.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(plan))
            {
                motivo = "Tiene sepelio pero no tiene un plan asignado.";
                return null;
            }

            if (nacimiento is null)
            {
                motivo = "Tiene sepelio pero no tiene fecha de nacimiento.";
                return null;
            }

            // El tramo "hasta" lleva el tope de edad; el tramo "más de" lo tiene en null.
            var delPlan = planesSepelio.Where(p => p.Tipo_Plan == plan).ToList();
            var tramoHasta = delPlan.FirstOrDefault(p => p.EdadTope.HasValue);
            var tramoMasDe = delPlan.FirstOrDefault(p => !p.EdadTope.HasValue);
            if (tramoHasta is null || tramoMasDe is null)
            {
                motivo = $"El plan '{plan}' no tiene la configuración de sepelio completa (tramos hasta/más de).";
                return null;
            }

            var edad = CalcularEdad(nacimiento.Value, hoy);

            // "hasta el tope" incluye la edad igual al tope; recién por encima pasa a "más de".
            motivo = null;
            return edad <= tramoHasta.EdadTope!.Value ? tramoHasta.TipoCuota : tramoMasDe.TipoCuota;
        }

        /// <summary>Arma una cuota nueva en estado PENDIENTE, sin movimiento asociado.</summary>
        private static Cuota NuevaCuota(
            int idEntidad, int idTipoCuota, decimal monto, DateTime periodo,
            DateTime vencimiento, DateTime fechaGeneracion)
        {
            return new Cuota
            {
                Id_Entidad = idEntidad,
                Mes_Periodo = periodo.Month,
                Anio_Periodo = periodo.Year,
                Id_TipoCuota = idTipoCuota,
                Monto = monto,
                FechaHoraGeneracion = fechaGeneracion,
                FechaVencimiento = vencimiento,
                Estado = EstadoPendiente,
                Id_Movimiento = null
            };
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
