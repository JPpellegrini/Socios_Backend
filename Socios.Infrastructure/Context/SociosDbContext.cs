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
        public DbSet<Cuenta> Cuenta { get; set; }
        public DbSet<DetalleMovimiento> DetalleMovimiento { get; set; }
        public DbSet<MetodoPago> MetodoPago { get; set; }
        public DbSet<ObraSocial> ObraSocial { get; set; }
        public DbSet<Prestacion> Prestacion { get; set; }
        public DbSet<TipoEntidad> TiposEntidad { get; set; }
        public DbSet<Codeudor> Codeudores { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<EntidadTipo> EntidadTipos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new RolConfiguration());
            modelBuilder.ApplyConfiguration(new CiudadConfiguration());
            modelBuilder.ApplyConfiguration(new EntidadConfiguration());
            modelBuilder.ApplyConfiguration(new CuentaConfiguration());
            modelBuilder.ApplyConfiguration(new DetalleMovimientoConfiguration());
            modelBuilder.ApplyConfiguration(new MetodoPagoConfiguration());
            modelBuilder.ApplyConfiguration(new ObraSocialConfiguration());
            modelBuilder.ApplyConfiguration(new PrestacionConfiguration());
            modelBuilder.ApplyConfiguration(new TipoEntidadConfiguration());
            modelBuilder.ApplyConfiguration(new CodeudorConfiguration());
            modelBuilder.ApplyConfiguration(new SocioConfiguration());
            modelBuilder.ApplyConfiguration(new EntidadTipoConfiguration());

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

            // Seed de Cuentas de prueba
            modelBuilder.Entity<Cuenta>().HasData(
                new Cuenta { Id_Cuenta = 1, Nombre = "Efectivo", NroCuenta = 0, Estado = "Activo" },
                new Cuenta { Id_Cuenta = 2, Nombre = "Banco Macro", NroCuenta = 789012, Estado = "Activo" },
                new Cuenta { Id_Cuenta = 3, Nombre = "Banco Macro 2", NroCuenta = 712906, Estado = "Inactivo" }
            );

            // Seed de Detalle de Movimientos de prueba
            modelBuilder.Entity<DetalleMovimiento>().HasData(
                new DetalleMovimiento { Id_Detmov = 1, NombreDetalleMovimiento = "Cuota Socio"},
                new DetalleMovimiento { Id_Detmov = 2, NombreDetalleMovimiento = "Cuota Sepelio" },
                new DetalleMovimiento { Id_Detmov = 3, NombreDetalleMovimiento = "Ajuste Diferencia Caja" },
                new DetalleMovimiento { Id_Detmov = 4, NombreDetalleMovimiento = "Colaboración" },
                new DetalleMovimiento { Id_Detmov = 5, NombreDetalleMovimiento = "Pago a Proveedor" },
                new DetalleMovimiento { Id_Detmov = 6, NombreDetalleMovimiento = "Transferencia" }

            );

            // Seed de Metodos de Pago de prueba
            modelBuilder.Entity<MetodoPago>().HasData(
                new MetodoPago { Id_MetodoPago = 1, NombreMetodoPago = "Efectivo" },
                new MetodoPago { Id_MetodoPago = 2, NombreMetodoPago = "Transferencia Bancaria" }
            );

            // Seed de Obras Sociales de prueba
            modelBuilder.Entity<ObraSocial>().HasData(
                new ObraSocial { Id_ObraSocial = 1, NombreObraSocial = "PAMI" },
                new ObraSocial { Id_ObraSocial = 2, NombreObraSocial = "OSDE" }
            );

            // Seed de Prestaciones de prueba
            modelBuilder.Entity<Prestacion>().HasData(
                new Prestacion { Id_Prestacion = 1, NombrePrestacion = "Podología" },
                new Prestacion { Id_Prestacion = 2, NombrePrestacion = "Cardiología" },
                new Prestacion { Id_Prestacion = 3, NombrePrestacion = "Alquiler Salón" },
                new Prestacion { Id_Prestacion = 4, NombrePrestacion = "Comisión Cobrador" }
            );

            // Seed de Tipo de Entidad de prueba
            modelBuilder.Entity<TipoEntidad>().HasData(
                new TipoEntidad { Id_Tipo = 1, NombreTipoEntidad = "Socio" },
                new TipoEntidad { Id_Tipo = 2, NombreTipoEntidad = "Proveedor" },
                new TipoEntidad { Id_Tipo = 3, NombreTipoEntidad = "Colaborador" },
                new TipoEntidad { Id_Tipo = 4, NombreTipoEntidad = "Empleado" }
            );

            // Seed de Socios de prueba
            modelBuilder.Entity<Socio>().HasData(
                new Socio { Id_Entidad = 1, Id_OS = 1, Plan = "A", Sepelio = "SI", Cobrador = "NO", Numero_Afiliado = "" }
            );

            // Seed de Codeudores de prueba
            modelBuilder.Entity<Codeudor>().HasData(
                new Codeudor { Id_Entidad_Codeudor = 1, Id_Entidad = 2 }
            );

            // Seed de EntidadTipo (PK compuesta) de prueba
            modelBuilder.Entity<EntidadTipo>().HasData(
                new EntidadTipo { Id_Entidad = 1, Id_Tipo = 1, Fecha_Alta = new System.DateTime(1995,1,18), Estado = "ACTIVO" },
                new EntidadTipo { Id_Entidad = 1, Id_Tipo = 3, Fecha_Alta = new System.DateTime(1995,1,18), Estado = "ACTIVO" },
                new EntidadTipo { Id_Entidad = 1978, Id_Tipo = 3, Fecha_Alta = new System.DateTime(1995,1,18), Estado = "INACTIVO" }
            );
        }
    }
}
