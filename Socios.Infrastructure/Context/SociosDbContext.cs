using Microsoft.EntityFrameworkCore;
using Socios.Domain.Entities;

namespace Socios.Infrastructure.Context
{
    public class SociosDbContext : DbContext
    {
        public SociosDbContext(DbContextOptions<SociosDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}
