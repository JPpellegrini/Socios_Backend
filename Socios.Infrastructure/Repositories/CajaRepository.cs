using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CajaRepository : ICajaRepository
    {
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
