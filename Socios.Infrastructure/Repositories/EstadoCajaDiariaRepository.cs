using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class EstadoCajaDiariaRepository : IEstadoCajaDiariaRepository
    {
        private const string TipoApertura = "APERTURA";
        private const string TipoCierre = "CIERRE";

        /// <summary>
        /// Clave del bloqueo consultivo de PostgreSQL (pg_advisory_xact_lock) sobre las cajas.
        /// Es un número arbitrario: solo tiene que ser el mismo en todos los lugares que lo usen.
        /// </summary>
        private const long ClaveBloqueoCajas = 7_301_001;

        private readonly SociosDbContext _context;

        public EstadoCajaDiariaRepository(SociosDbContext context)
        {
            _context = context;
        }

        public async Task<EstadoCajaDiaria?> ObtenerCajaAbiertaAsync(int idUsuario)
        {
            // La tabla registra eventos (APERTURA / CIERRE). La caja del usuario está abierta si
            // su último evento es una APERTURA; los movimientos se cuelgan de esa fila.
            var ultimo = await _context.EstadosCajaDiaria
                .AsNoTracking()
                .Where(c => c.Id_Usuario == idUsuario)
                .OrderByDescending(c => c.FechaHora)
                .ThenByDescending(c => c.Id_CajaDiaria)
                .FirstOrDefaultAsync();

            return ultimo is not null && ultimo.Tipo == TipoApertura ? ultimo : null;
        }

        public async Task<EstadoCajaDiaria?> ObtenerUltimoCierreAsync(int idUsuario)
        {
            return await _context.EstadosCajaDiaria
                .AsNoTracking()
                .Where(c => c.Id_Usuario == idUsuario && c.Tipo == TipoCierre)
                .OrderByDescending(c => c.FechaHora)
                .ThenByDescending(c => c.Id_CajaDiaria)
                .FirstOrDefaultAsync();
        }

        public async Task BloquearCajasAsync()
        {
            // Bloqueo a nivel de transacción: se libera solo al confirmar o deshacer. No toca
            // filas ni tablas; solo coordina a quienes pidan el mismo bloqueo.
            await _context.Database.ExecuteSqlRawAsync(
                "SELECT pg_advisory_xact_lock({0})", ClaveBloqueoCajas);
        }

        public async Task BloquearCajasCompartidoAsync()
        {
            // Versión compartida: varios cobros a la vez no se bloquean entre sí, pero no
            // conviven con una apertura/cierre (que toma el exclusivo).
            await _context.Database.ExecuteSqlRawAsync(
                "SELECT pg_advisory_xact_lock_shared({0})", ClaveBloqueoCajas);
        }

        public async Task<int> ObtenerSiguienteNroCajaAsync()
        {
            var maximo = await _context.EstadosCajaDiaria
                .MaxAsync(c => (int?)c.Nro_Caja);

            return (maximo ?? 0) + 1;
        }

        public void Agregar(EstadoCajaDiaria registro)
        {
            // Solo marca el registro para insertar. El guardado real lo dispara la unidad de trabajo.
            _context.EstadosCajaDiaria.Add(registro);
        }
    }
}
