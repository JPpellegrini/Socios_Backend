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
        public DbSet<Ciudad> Ciudades { get; set; }
        public DbSet<Entidad> Entidades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new RolConfiguration());
            modelBuilder.ApplyConfiguration(new CiudadConfiguration());
            modelBuilder.ApplyConfiguration(new EntidadConfiguration());

            // Seed de Rol Secretaria
            modelBuilder.Entity<Rol>().HasData(new Rol
            {
                Id_Rol = 1,
                RolNombre = "Secretaria",
                Descripcion = "secretaria que maneja todo el sistema"
            });

            // Seed de Usuario de prueba (password: 1234, hash con BCrypt)
            modelBuilder.Entity<Usuario>().HasData(new Usuario
            {
                Id_Usuario = 1,
                UsuarioNombre = "CJR",
                Password = "$2a$11$5.O9NB.FFBZ98GrE24jq7et8c0ACOkRVsSueihm78or/JNZmjVGay", // hash de "1234"
                Estado = "Activo",
                Id_Rol = 1
            });

            // Seed de Ciudades de prueba
            modelBuilder.Entity<Ciudad>().HasData(
                new Ciudad { Id_Ciudad = 1, Nombre = "Roldán" },
                new Ciudad { Id_Ciudad = 2, Nombre = "Funes" }
            );

            // Seed de Entidades de ejemplo
            modelBuilder.Entity<Entidad>().HasData(
                new Entidad
                {
                    Id_Entidad = 1,
                    Tipo = "DNI",
                    Dni = "12345678",
                    CuitCuil = "20123456784",
                    Nombre = "Luciano",
                    Apellido = "Oldan",
                    RazonSocial = null,
                    Sexo = "Hombre",
                    Nacimiento = new System.DateTime(1980, 1, 1),
                    Id_Ciudad = 1,
                    Calle = "Independencia",
                    Altura = 250,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 2,
                    Tipo = "DNI",
                    Dni = "12345678",
                    CuitCuil = "20123456784",
                    Nombre = null,
                    Apellido = null,
                    RazonSocial = "CJR",
                    Sexo = "Persona Juridica",
                    Nacimiento = new System.DateTime(1980, 1, 1),
                    Id_Ciudad = 1,
                    Calle = "Independencia",
                    Altura = 250,
                    Observacion = null
                }
            );
        }
    }
}
