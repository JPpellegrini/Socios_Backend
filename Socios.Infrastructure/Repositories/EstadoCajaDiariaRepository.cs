using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class EstadoCajaDiariaRepository : IEstadoCajaDiariaRepository
    {
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

            return ultimo is not null && ultimo.Tipo == "APERTURA" ? ultimo : null;
        }
    }
}
