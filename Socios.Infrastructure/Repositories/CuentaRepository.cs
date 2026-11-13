using Microsoft.EntityFrameworkCore;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CuentaRepository : ICuentaRepository
    {
        private readonly SociosDbContext _context;

        public CuentaRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<Cuenta?> ObtenerPorIdAsync(int idCuenta)
        {
            return await _context.Cuenta
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id_Cuenta == idCuenta);
        }

        public async Task<Cuenta?> ObtenerPorNombreAsync(string nombreCuenta)
        {
            return await _context.Cuenta
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.NombreCuenta == nombreCuenta);
        }

        public async Task<List<CuentaSaldoDto>> ObtenerSaldosAsync()
        {
            // Una sola consulta: por cada cuenta, la suma de ingresos y egresos de sus movimientos
            // desde la fecha del saldo inicial (todos, si no tiene fecha).
            var saldos = await _context.Cuenta
                .AsNoTracking()
                .OrderBy(c => c.NombreCuenta)
                .Select(c => new
                {
                    Cuenta = c,
                    Ingresos = _context.Caja
                        .Where(m => m.Id_Cuenta == c.Id_Cuenta
                                    && m.Tipo_Movimiento == "INGRESO"
                                    && (c.FechaSaldoInicial == null || m.FechaHoraMov >= c.FechaSaldoInicial))
                        .Sum(m => (decimal?)m.Monto) ?? 0m,
                    Egresos = _context.Caja
                        .Where(m => m.Id_Cuenta == c.Id_Cuenta
                                    && m.Tipo_Movimiento == "EGRESO"
                                    && (c.FechaSaldoInicial == null || m.FechaHoraMov >= c.FechaSaldoInicial))
                        .Sum(m => (decimal?)m.Monto) ?? 0m
                })
                .ToListAsync();

            return saldos
                .Select(s => new CuentaSaldoDto
                {
                    IdCuenta = s.Cuenta.Id_Cuenta,
                    NombreCuenta = s.Cuenta.NombreCuenta,
                    NroCuenta = s.Cuenta.NroCuenta,
                    Estado = s.Cuenta.Estado,
                    SaldoInicial = s.Cuenta.SaldoInicial,
                    FechaSaldoInicial = s.Cuenta.FechaSaldoInicial,
                    Ingresos = s.Ingresos,
                    Egresos = s.Egresos,
                    Saldo = s.Cuenta.SaldoInicial + s.Ingresos - s.Egresos
                })
                .ToList();
        }

        public async Task<List<Cuenta>> ListarActivasAsync()
        {
            return await _context.Cuenta
                .AsNoTracking()
                .Where(c => c.Estado == "ACTIVA")
                .OrderBy(c => c.NombreCuenta)
                .ToListAsync();
        }
    }
}
