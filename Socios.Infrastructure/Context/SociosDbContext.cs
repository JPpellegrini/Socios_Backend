using Microsoft.EntityFrameworkCore;
using Socios.Domain.Entities;
using Socios.Infrastructure.Configurations;

namespace Socios.Infrastructure.Context
{
    public class SociosDbContext : DbContext
    {
        public SociosDbContext(DbContextOptions<SociosDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new RolConfiguration());

            // Seed de Rol Secretaria
            modelBuilder.Entity<Rol>().HasData(new Rol
            {
                Id_Rol = 1,
                RolNombre = "Secretaria",
                Descripcion = "secretaria que maneja todo el sistema"
            });

            // Seed de Usuario de prueba
            modelBuilder.Entity<Usuario>().HasData(new Usuario
            {
                Id_Usuario = 1,
                UsuarioNombre = "CJR",
                Password = "1234",
                Estado = "Activo",
                Id_Rol = 1
            });
        }
    }
}
