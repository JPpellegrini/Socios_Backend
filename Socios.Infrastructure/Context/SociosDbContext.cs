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
        public DbSet<EstadoCajaDiaria> EstadosCajaDiaria { get; set; }
        public DbSet<Caja> Caja { get; set; }
        public DbSet<Codeudor> Codeudores { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<EntidadTipo> EntidadTipos { get; set; }
        public DbSet<EntidadBaja> EntidadBajas { get; set; }
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Contacto> Contactos { get; set; }

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
            modelBuilder.ApplyConfiguration(new EstadoCajaDiariaConfiguration());
            modelBuilder.ApplyConfiguration(new CajaConfiguration());
            modelBuilder.ApplyConfiguration(new CodeudorConfiguration());
            modelBuilder.ApplyConfiguration(new SocioConfiguration());
            modelBuilder.ApplyConfiguration(new EntidadTipoConfiguration());
            modelBuilder.ApplyConfiguration(new EntidadBajaConfiguration());
            modelBuilder.ApplyConfiguration(new ColaboradorConfiguration());
            modelBuilder.ApplyConfiguration(new ContactoConfiguration());

            // Seed de Rol Secretaria
            modelBuilder.Entity<Rol>().HasData(new Rol
            {
                Id_Rol = 1,
                RolNombre = "Secretaria",
                Descripcion = "Secretaria que maneja todo el sistema"
            },
            new Rol
            {
                Id_Rol = 2,
                RolNombre = "Consultor",
                Descripcion = "Solo posee acceso al módulo de informes"
            }
            );

            // Seed de Usuario de prueba (password: 1234, hash con BCrypt)
            modelBuilder.Entity<Usuario>().HasData(new Usuario
            {
                Id_Usuario = 1,
                UsuarioNombre = "CJR",
                Password = "$2a$11$5.O9NB.FFBZ98GrE24jq7et8c0ACOkRVsSueihm78or/JNZmjVGay", // hash de "1234"
                Estado = "Activo",
                Id_Rol = 2
            },
            new Usuario          
            {
                Id_Usuario = 2,
                UsuarioNombre = "Adriana",
                Password = "$2a$11$5.O9NB.FFBZ98GrE24jq7et8c0ACOkRVsSueihm78or/JNZmjVGay", // hash de "1234"
                Estado = "Activo",
                Id_Rol = 1
            },
            new Usuario
            {
                Id_Usuario = 3,
                UsuarioNombre = "Marcela",
                Password = "$2a$11$5.O9NB.FFBZ98GrE24jq7et8c0ACOkRVsSueihm78or/JNZmjVGay", // hash de "1234"
                Estado = "Activo",
                Id_Rol = 1
            }
            );

            // Seed de Ciudades de prueba
            modelBuilder.Entity<Ciudad>().HasData(
                new Ciudad { Id_Ciudad = 1, Nombre = "Roldán" },
                new Ciudad { Id_Ciudad = 2, Nombre = "Funes" },
                new Ciudad { Id_Ciudad = 3, Nombre = "Rosario" }
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
                    Dni = null,
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
                },
                new Entidad
                {
                    Id_Entidad = 3,
                    Tipo = "DNI",
                    Dni = null,
                    CuitCuil = "30657866330",
                    Nombre = null,
                    Apellido = null,
                    RazonSocial = "Litoral Gas S.A",
                    Sexo = "Persona Juridica",
                    Nacimiento = new System.DateTime(1980, 1, 1),
                    Id_Ciudad = 3,
                    Calle = "Mitre",
                    Altura = 166,
                    Observacion = null
                }
            );

            // Seed de Cuentas de prueba
            modelBuilder.Entity<Cuenta>().HasData(
                new Cuenta { Id_Cuenta = 1, NombreCuenta = "Efectivo", NroCuenta = 0, Estado = "Activo" },
                new Cuenta { Id_Cuenta = 2, NombreCuenta = "Banco Macro", NroCuenta = 789012, Estado = "Activo" },
                new Cuenta { Id_Cuenta = 3, NombreCuenta = "Banco Macro 2", NroCuenta = 712906, Estado = "Inactivo" }
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

            // Seed de EstadoCajaDiaria de prueba
            modelBuilder.Entity<EstadoCajaDiaria>().HasData(
                new EstadoCajaDiaria { Id_CajaDiaria = 1, 
                                       Id_Usuario = 1, 
                                       Tipo = "Apertura", 
                                       FechaHora = new DateTime(2026, 6, 9, 08, 00, 0), 
                                       Saldo = 0 },
                new EstadoCajaDiaria { Id_CajaDiaria = 2, 
                                       Id_Usuario = 1, 
                                       Tipo = "Cierre", 
                                       FechaHora = new DateTime(2026, 6, 9, 12, 05, 0), 
                                       Saldo = 150000 }
            );

            // Seed de Caja de prueba
            modelBuilder.Entity<Caja>().HasData(
                new Caja
                {
                    Id_Movimiento = 1,
                    Id_CajaDiaria = 1,
                    Id_Usuario = 2,
                    FechaHoraMov = new DateTime(2026, 6, 10, 08, 20, 0),
                    Tipo_Movimiento = "Ingreso",
                    Id_Entidad = 1,
                    Monto = 13500,
                    Id_MetodoPago = 2,
                    Id_Cuenta = 1,
                    Id_Detmov = 1,
                    Observacion = "Pago Cuota Mayo"
                },
                new Caja
                {
                    Id_Movimiento = 2,
                    Id_CajaDiaria = 1,
                    Id_Usuario = 2,
                    FechaHoraMov = new DateTime(2026, 6, 10, 09, 30, 0),
                    Tipo_Movimiento = "Egreso",
                    Id_Entidad = 3,
                    Monto = 42500.47m,
                    Id_MetodoPago = 1,
                    Id_Cuenta = 2,
                    Id_Detmov = 5,
                    Observacion = "Pago Cuota Mayo"
                }
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

            // Seed de EntidadBajas
            modelBuilder.Entity<EntidadBaja>().HasData(
                new EntidadBaja { Id_Baja = 1, Id_Entidad = 1978, Id_Tipo = 1, Fecha_Baja = new System.DateTime(2016,1,18), Motivo = "MORA" },
                new EntidadBaja { Id_Baja = 2, Id_Entidad = 201, Id_Tipo = 3, Fecha_Baja = new System.DateTime(2015,12,11), Motivo = "FALLECIMIENTO" },
                new EntidadBaja { Id_Baja = 3, Id_Entidad = 1458, Id_Tipo = 3, Fecha_Baja = new System.DateTime(2016,2,21), Motivo = "RENUNCIA" }
            );

            // Seed de Colaboradores
            modelBuilder.Entity<Colaborador>().HasData(
                new Colaborador { Id_Colaborador = 1, Id_Prestacion = 1, Id_Entidad = 1 },
                new Colaborador { Id_Colaborador = 2, Id_Prestacion = 3, Id_Entidad = 2 }
            );

            // Seed de Contactos
            modelBuilder.Entity<Contacto>().HasData(
                new Contacto { Id_Contacto = 1, Tipo = "Celular", ContactoValor = "1234", Id_Entidad = 1 },
                new Contacto { Id_Contacto = 2, Tipo = "Mail", ContactoValor = "luciano@gmail", Id_Entidad = 1 }
            );
        }
    }
}
