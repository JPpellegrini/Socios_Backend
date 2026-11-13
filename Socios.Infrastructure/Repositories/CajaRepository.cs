using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CajaRepository : ICajaRepository
    {
        private const string TipoIngreso = "INGRESO";
        private const string TipoEgreso = "EGRESO";
        private const string DetalleTransferencia = "TRANSFERENCIA";

        /// <summary>Marca con la que la transferencia agrega el comprobante bancario a la observación.</summary>
        private const string MarcaComprobante = " - COMP. ";

        private readonly SociosDbContext _context;

        public CajaRepository(SociosDbContext context)
        {
            _context = context;
        }

        public void Agregar(Caja movimiento)
        {
            // Solo marca el movimiento para insertar. El guardado real lo dispara la unidad de trabajo.
            _context.Caja.Add(movimiento);
        }

        public async Task<(decimal Ingresos, decimal Egresos)> ObtenerTotalesPorCuentaAsync(int idCajaDiaria, int idCuenta)
        {
            var movimientos = _context.Caja
                .AsNoTracking()
                .Where(m => m.Id_CajaDiaria == idCajaDiaria && m.Id_Cuenta == idCuenta);

            var ingresos = await movimientos
                .Where(m => m.Tipo_Movimiento == TipoIngreso)
                .SumAsync(m => (decimal?)m.Monto) ?? 0m;
            var egresos = await movimientos
                .Where(m => m.Tipo_Movimiento == TipoEgreso)
                .SumAsync(m => (decimal?)m.Monto) ?? 0m;

            return (ingresos, egresos);
        }

        public async Task<ComprobanteTransferenciaDto?> ObtenerComprobanteTransferenciaAsync(int idMovimiento)
        {
            var movimiento = await _context.Caja
                .AsNoTracking()
                .Include(m => m.DetalleMovimiento)
                .FirstOrDefaultAsync(m => m.Id_Movimiento == idMovimiento);

            if (movimiento is null || movimiento.DetalleMovimiento.NombreDetalleMovimiento != DetalleTransferencia)
                return null;

            // La transferencia no guarda un vínculo explícito entre sus dos movimientos: su par es
            // el del tipo opuesto registrado en la MISMA caja, con la misma fecha/hora, importe y
            // observación (se graban juntos en una transacción). Si hubiera más de uno, el de Id
            // más cercano.
            var tipoPar = movimiento.Tipo_Movimiento == TipoIngreso ? TipoEgreso : TipoIngreso;
            var candidatos = await _context.Caja
                .AsNoTracking()
                .Where(m => m.Id_Movimiento != movimiento.Id_Movimiento
                            && m.Id_CajaDiaria == movimiento.Id_CajaDiaria
                            && m.Id_Detmov == movimiento.Id_Detmov
                            && m.Tipo_Movimiento == tipoPar
                            && m.FechaHoraMov == movimiento.FechaHoraMov
                            && m.Monto == movimiento.Monto
                            && m.Observacion == movimiento.Observacion)
                .Select(m => m.Id_Movimiento)
                .ToListAsync();

            if (candidatos.Count == 0)
                return null;

            var idPar = candidatos.MinBy(id => Math.Abs(id - movimiento.Id_Movimiento));
            var idEgreso = movimiento.Tipo_Movimiento == TipoEgreso ? movimiento.Id_Movimiento : idPar;
            var idIngreso = movimiento.Tipo_Movimiento == TipoIngreso ? movimiento.Id_Movimiento : idPar;

            var comprobante = await (
                from eg in _context.Caja
                from ing in _context.Caja.Where(i => i.Id_Movimiento == idIngreso)
                join co in _context.Cuenta on eg.Id_Cuenta equals co.Id_Cuenta
                join cd in _context.Cuenta on ing.Id_Cuenta equals cd.Id_Cuenta
                join mp in _context.MetodoPago on eg.Id_MetodoPago equals mp.Id_MetodoPago
                join caja in _context.EstadosCajaDiaria on eg.Id_CajaDiaria equals caja.Id_CajaDiaria
                join u in _context.Usuarios on caja.Id_Usuario equals u.Id_Usuario
                where eg.Id_Movimiento == idEgreso
                select new ComprobanteTransferenciaDto
                {
                    IdMovimientoEgreso = eg.Id_Movimiento,
                    IdMovimientoIngreso = ing.Id_Movimiento,
                    FechaHora = eg.FechaHoraMov,
                    Importe = eg.Monto,
                    MetodoPago = mp.NombreMetodoPago,
                    CuentaOrigen = co.NombreCuenta,
                    NroCuentaOrigen = co.NroCuenta,
                    CuentaDestino = cd.NombreCuenta,
                    NroCuentaDestino = cd.NroCuenta,
                    ComprobanteBancario = eg.Observacion,
                    Cajero = u.UsuarioNombre,
                    NroCaja = caja.Nro_Caja
                }
            ).AsNoTracking().FirstOrDefaultAsync();

            if (comprobante is not null)
                comprobante.ComprobanteBancario = ExtraerComprobanteBancario(comprobante.ComprobanteBancario);

            return comprobante;
        }

        /// <summary>
        /// La transferencia guarda el comprobante bancario dentro de la observación:
        /// "TRANSFERENCIA EFECTIVO A BANCO MACRO - COMP. 123456" → "123456". Null si no tiene.
        /// </summary>
        private static string? ExtraerComprobanteBancario(string? observacion)
        {
            if (string.IsNullOrEmpty(observacion))
                return null;

            var posicion = observacion.LastIndexOf(MarcaComprobante, StringComparison.Ordinal);
            if (posicion < 0)
                return null;

            var numero = observacion[(posicion + MarcaComprobante.Length)..].Trim();
            return numero.Length > 0 ? numero : null;
        }

        public async Task<ComprobantePagoDto?> ObtenerComprobantePagoAsync(int idMovimiento)
        {
            // El movimiento corresponde al cobro de una cuota si alguna cuota lo referencia.
            // De ahí salen la cuota (concepto, período) y el socio; de la caja diaria, quién cobró.
            return await (
                from m in _context.Caja
                join q in _context.Cuotas on m.Id_Movimiento equals q.Id_Movimiento
                join tc in _context.TiposCuotas on q.Id_TipoCuota equals tc.Id_TipoCuota
                join s in _context.Socios on q.Id_Entidad equals s.Id_Entidad
                join e in _context.Entidades on q.Id_Entidad equals e.Id_Entidad
                join ci in _context.Ciudades on e.Id_Ciudad equals ci.Id_Ciudad into ciudades
                from ci in ciudades.DefaultIfEmpty()
                join mp in _context.MetodoPago on m.Id_MetodoPago equals mp.Id_MetodoPago
                join cu in _context.Cuenta on m.Id_Cuenta equals cu.Id_Cuenta
                join cd in _context.EstadosCajaDiaria on m.Id_CajaDiaria equals cd.Id_CajaDiaria
                join u in _context.Usuarios on cd.Id_Usuario equals u.Id_Usuario
                where m.Id_Movimiento == idMovimiento
                select new ComprobantePagoDto
                {
                    IdMovimiento = m.Id_Movimiento,
                    FechaHoraPago = m.FechaHoraMov,
                    Monto = m.Monto,
                    MetodoPago = mp.NombreMetodoPago,
                    Cuenta = cu.NombreCuenta,
                    IdCuota = q.Id_Deuda,
                    Concepto = tc.Concepto,
                    Mes = q.Mes_Periodo,
                    Anio = q.Anio_Periodo,
                    IdSocio = s.Id_Socio,
                    Dni = e.Dni,
                    Apellido = e.Apellido,
                    Nombre = e.Nombre,
                    Calle = e.Calle,
                    Altura = e.Altura,
                    Ciudad = ci != null ? ci.Nombre : null,
                    Cajero = u.UsuarioNombre,
                    NroCaja = cd.Nro_Caja
                }
            ).AsNoTracking().FirstOrDefaultAsync();
        }
    }
}
