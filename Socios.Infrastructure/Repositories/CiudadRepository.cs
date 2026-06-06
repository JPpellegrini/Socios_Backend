using Microsoft.EntityFrameworkCore;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;
using Socios.Infrastructure.Context;

namespace Socios.Infrastructure.Repositories
{
    public class CiudadRepository : ICiudadRepository
    {
        private readonly SociosDbContext _context;

        public CiudadRepository(SociosDbContext context)
        {
            _context = context;
        }
    }
}
