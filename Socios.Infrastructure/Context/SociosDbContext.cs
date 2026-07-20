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
        public DbSet<MovimientoPrestacion> MovimientoPrestaciones { get; set; }
        public DbSet<Nicho> Nichos { get; set; }
        public DbSet<CierreContable> CierresContables { get; set; }
        public DbSet<TipoCuota> TiposCuotas { get; set; }
        public DbSet<Cuota> Cuotas { get; set; }
        public DbSet<TipoPlan> TiposPlan { get; set; }

        /// <summary>
        /// Convención global: todos los DateTime se mapean a 'timestamp without time zone'
        /// (hora local del negocio, sin requerir UTC). Las propiedades que representan solo
        /// una fecha (nacimiento, alta, baja, vencimiento) se sobrescriben a 'date' en sus
        /// respectivas configuraciones.
        /// </summary>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp without time zone");
        }

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
            modelBuilder.ApplyConfiguration(new MovimientoPrestacionConfiguration());
            modelBuilder.ApplyConfiguration(new NichoConfiguration());
            modelBuilder.ApplyConfiguration(new CierreContableConfiguration());
            modelBuilder.ApplyConfiguration(new TipoCuotaConfiguration());
            modelBuilder.ApplyConfiguration(new CuotaConfiguration());
            modelBuilder.ApplyConfiguration(new TipoPlanConfiguration());

            // Seed de Rol Secretaria de prueba
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
                    Dni = "32127057",
                    CuitCuil = "20321270574",
                    Nombre = "Luciano",
                    Apellido = "Oldan",
                    RazonSocial = null,
                    Sexo = "Hombre",
                    Nacimiento = new DateTime(1959, 7, 12, 0, 0, 0),
                    Id_Ciudad = 1,
                    Calle = "Independencia",
                    Altura = 250,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 2,
                    Tipo = "DNI",
                    Dni = "32185166",
                    CuitCuil = null,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    RazonSocial = null,
                    Sexo = "Hombre",
                    Nacimiento = new DateTime(1998, 3, 1),
                    Id_Ciudad = 2,
                    Calle = "Galindo",
                    Altura = 458,
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
                    Nacimiento = new DateTime(1980, 1, 1),
                    Id_Ciudad = 3,
                    Calle = "Mitre",
                    Altura = 166,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 4,
                    Tipo = "DNI",
                    Dni = "55406681",
                    CuitCuil = null,
                    Nombre = "María",
                    Apellido = "López",
                    RazonSocial = null,
                    Sexo = "Mujer",
                    Nacimiento = new DateTime(1987, 12, 11),
                    Id_Ciudad = 1,
                    Calle = "Las Heras",
                    Altura = 347,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 5,
                    Tipo = "DNI",
                    Dni = "6166108",
                    CuitCuil = null,
                    Nombre = "EDUARDO",
                    Apellido = "AVILA",
                    RazonSocial = null,
                    Sexo = "Hombre",
                    Nacimiento = new DateTime(1958, 4, 12),
                    Id_Ciudad = 1,
                    Calle = "Salta",
                    Altura = 417,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 6,
                    Tipo = "DNI",
                    Dni = "6164804",
                    CuitCuil = null,
                    Nombre = "RUTILIO ALFREDO",
                    Apellido = "MASABEU",
                    RazonSocial = null,
                    Sexo = "Hombre",
                    Nacimiento = new DateTime(1960, 7, 25),
                    Id_Ciudad = 1,
                    Calle = "Independencia",
                    Altura = 270,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 7,
                    Tipo = "DNI",
                    Dni = "14658096",
                    CuitCuil = null,
                    Nombre = "ALFREDO",
                    Apellido = "MASABEU",
                    RazonSocial = null,
                    Sexo = "Hombre",
                    Nacimiento = new DateTime(1955, 11, 3),
                    Id_Ciudad = 1,
                    Calle = "Mendoza",
                    Altura = 684,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 8,
                    Tipo = "DNI",
                    Dni = "4989850",
                    CuitCuil = null,
                    Nombre = "JOSÉ MOISÉS",
                    Apellido = "MEYADO",
                    RazonSocial = null,
                    Sexo = "Hombre",
                    Nacimiento = new DateTime(1962, 2, 18),
                    Id_Ciudad = 1,
                    Calle = "Corrientes",
                    Altura = 867,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 9,
                    Tipo = "DNI",
                    Dni = "6176088",
                    CuitCuil = null,
                    Nombre = "HUGO",
                    Apellido = "IPPOLITI",
                    RazonSocial = null,
                    Sexo = "Hombre",
                    Nacimiento = new DateTime(1959, 9, 30),
                    Id_Ciudad = 1,
                    Calle = "Mendoza",
                    Altura = 1044,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 10,
                    Tipo = "DNI",
                    Dni = "3247480",
                    CuitCuil = null,
                    Nombre = "SUSANA",
                    Apellido = "FLORIÁN DE AVILA",
                    RazonSocial = null,
                    Sexo = "Mujer",
                    Nacimiento = new DateTime(1961, 5, 5),
                    Id_Ciudad = 1,
                    Calle = "Salta",
                    Altura = 417,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 11,
                    Tipo = "DNI",
                    Dni = "3962438",
                    CuitCuil = null,
                    Nombre = "LUCY",
                    Apellido = "HERRERA",
                    RazonSocial = null,
                    Sexo = "Mujer",
                    Nacimiento = new DateTime(1956, 12, 4),
                    Id_Ciudad = 1,
                    Calle = "1 de Mayo",
                    Altura = 292,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 12,
                    Tipo = "DNI",
                    Dni = "4105793",
                    CuitCuil = null,
                    Nombre = "IRIS A.",
                    Apellido = "MURATTURE",
                    RazonSocial = null,
                    Sexo = "Mujer",
                    Nacimiento = new DateTime(1957, 5, 5),
                    Id_Ciudad = 1,
                    Calle = "Mitre",
                    Altura = 369,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 13,
                    Tipo = "DNI",
                    Dni = "12266301",
                    CuitCuil = null,
                    Nombre = "RAQUEL",
                    Apellido = "VILLAREAL",
                    RazonSocial = null,
                    Sexo = "Mujer",
                    Nacimiento = new DateTime(1963, 4, 6),
                    Id_Ciudad = 1,
                    Calle = "Jujuy",
                    Altura = 455,
                    Observacion = null
                },
                new Entidad
                {
                    Id_Entidad = 14,
                    Tipo = "DNI",
                    Dni = "4645560",
                    CuitCuil = null,
                    Nombre = "ANA MARÍA",
                    Apellido = "BRAVO DE PONTI",
                    RazonSocial = null,
                    Sexo = "Mujer",
                    Nacimiento = new DateTime(1964, 10, 8),
                    Id_Ciudad = 1,
                    Calle = "Jujuy",
                    Altura = 953,
                    Observacion = null
                }
            );

            // Seed de Cuentas de prueba
            modelBuilder.Entity<Cuenta>().HasData(
                new Cuenta 
                { 
                    Id_Cuenta = 1, 
                    NombreCuenta = "Efectivo", 
                    NroCuenta = 0, 
                    Estado = "Activa" 
                },
                new Cuenta 
                { 
                    Id_Cuenta = 2, 
                    NombreCuenta = "Banco Macro", 
                    NroCuenta = 789012, 
                    Estado = "Activa" 
                },
                new Cuenta 
                { 
                    Id_Cuenta = 3, 
                    NombreCuenta = "Banco Macro 2", 
                    NroCuenta = 712906, 
                    Estado = "Inactiva" 
                }
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
                new EstadoCajaDiaria 
                { 
                    Id_CajaDiaria = 1, 
                    Nro_Caja = 1,
                    Id_Usuario = 1, 
                    Tipo = "Apertura", 
                    FechaHora = new DateTime(2026, 6, 9, 8, 00, 0), 
                    Saldo = 100000 
                },
                new EstadoCajaDiaria 
                {   
                    Id_CajaDiaria = 2, 
                    Nro_Caja = 1,
                    Id_Usuario = 1, 
                    Tipo = "Cierre", 
                    FechaHora = new DateTime(2026, 6, 9, 12, 05, 0), 
                    Saldo = 142000 
                },
                new EstadoCajaDiaria
                {
                    Id_CajaDiaria = 3,
                    Nro_Caja = 2,
                    Id_Usuario = 2,
                    Tipo = "Apertura",
                    FechaHora = new DateTime(2026, 6, 9, 8, 10, 0),
                    Saldo = 78000
                },
                new EstadoCajaDiaria
                {
                    Id_CajaDiaria = 4,
                    Nro_Caja = 2,
                    Id_Usuario = 2,
                    Tipo = "Cierre",
                    FechaHora = new DateTime(2026, 6, 9, 12, 15, 0),
                    Saldo = 250000
                }
            );

            // Seed de Caja de prueba
            modelBuilder.Entity<Caja>().HasData(
                new Caja
                {
                    Id_Movimiento = 1,
                    Id_CajaDiaria = 1,
                    FechaHoraMov = new DateTime(2026, 6, 10, 8, 20, 0),
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
                    FechaHoraMov = new DateTime(2026, 6, 10, 9, 30, 0),
                    Tipo_Movimiento = "Egreso",
                    Id_Entidad = 3,
                    Monto = 42500.47m,
                    Id_MetodoPago = 1,
                    Id_Cuenta = 2,
                    Id_Detmov = 5,
                    Observacion = "Pago Gas Mes Junio"
                },
                new Caja
                {
                    Id_Movimiento = 3,
                    Id_CajaDiaria = 2,
                    FechaHoraMov = new DateTime(2026, 6, 10, 09, 30, 0),
                    Tipo_Movimiento = "Ingreso",
                    Id_Entidad = 4,
                    Monto = 30000,
                    Id_MetodoPago = 1,
                    Id_Cuenta = 2,
                    Id_Detmov = 4,
                    Observacion = "Colaboración Podología"
                }
            );    

            // Seed de Socios de prueba
            modelBuilder.Entity<Socio>().HasData(
                new Socio { Id_Socio = 1, Id_Entidad = 1, Id_OS = 1, Plan = "A", Sepelio = "SI", Cobrador = "NO", Numero_Afiliado = "" },
                new Socio { Id_Socio = 4, Id_Entidad = 5, Id_OS = 1, Plan = "A", Sepelio = "SI", Cobrador = "NO", Numero_Afiliado = "" },
                new Socio { Id_Socio = 5, Id_Entidad = 6, Id_OS = 1, Plan = "B", Sepelio = "NO", Cobrador = "SI", Numero_Afiliado = "" },
                new Socio { Id_Socio = 6, Id_Entidad = 7, Id_OS = 1, Plan = "A", Sepelio = "NO", Cobrador = "NO", Numero_Afiliado = "" },
                new Socio { Id_Socio = 7, Id_Entidad = 8, Id_OS = 1, Plan = "B", Sepelio = "SI", Cobrador = "SI", Numero_Afiliado = "" },
                new Socio { Id_Socio = 8, Id_Entidad = 9, Id_OS = 1, Plan = "A", Sepelio = "SI", Cobrador = "SI", Numero_Afiliado = "" },
                new Socio { Id_Socio = 9, Id_Entidad = 10, Id_OS = 1, Plan = "B", Sepelio = "NO", Cobrador = "NO", Numero_Afiliado = "" },
                new Socio { Id_Socio = 10, Id_Entidad = 11, Id_OS = 1, Plan = "A", Sepelio = "NO", Cobrador = "SI", Numero_Afiliado = "" },
                new Socio { Id_Socio = 11, Id_Entidad = 12, Id_OS = 1, Plan = "B", Sepelio = "SI", Cobrador = "NO", Numero_Afiliado = "" },
                new Socio { Id_Socio = 12, Id_Entidad = 13, Id_OS = 1, Plan = "A", Sepelio = "SI", Cobrador = "NO", Numero_Afiliado = "" },
                new Socio { Id_Socio = 13, Id_Entidad = 14, Id_OS = 1, Plan = "B", Sepelio = "NO", Cobrador = "SI", Numero_Afiliado = "" }
            );

            // Seed de Codeudores de prueba
            modelBuilder.Entity<Codeudor>().HasData(
                new Codeudor 
                { 
                    Id_Codeudor = 1, 
                    Id_EntidadCodeudor = 1, 
                    Id_Entidad = 2 
                }
            );

            // Seed de EntidadTipo de prueba
            modelBuilder.Entity<EntidadTipo>().HasData(
                new EntidadTipo 
                {   
                    Id_EntidadTipo = 1, 
                    Id_Entidad = 1, 
                    Id_Tipo = 1, 
                    Fecha_Alta = new DateTime(1995,1,18, 0, 0, 0), 
                    Estado = "INACTIVO" 
                },
                new EntidadTipo 
                { 
                    Id_EntidadTipo = 2, 
                    Id_Entidad = 1, 
                    Id_Tipo = 3, 
                    Fecha_Alta = new DateTime(2000,11,18, 0, 0, 0), 
                    Estado = "ACTIVO" 
                },
                new EntidadTipo 
                { 
                    Id_EntidadTipo = 3, 
                    Id_Entidad = 4, 
                    Id_Tipo = 3, 
                    Fecha_Alta = new DateTime(2008,1,18, 0, 0, 0), 
                    Estado = "INACTIVO" 
                },
                                new EntidadTipo 
                {   
                    Id_EntidadTipo = 4, 
                    Id_Entidad = 5, 
                    Id_Tipo = 1, 
                    Fecha_Alta = new DateTime(1995,1,18, 0, 0, 0), 
                    Estado = "ACTIVO" 
                },
                new EntidadTipo
                {
                    Id_EntidadTipo = 5,
                    Id_Entidad = 6,
                    Id_Tipo = 1,
                    Fecha_Alta = new DateTime(2020, 3, 15),
                    Estado = "ACTIVO"
                },
                new EntidadTipo
                {
                    Id_EntidadTipo = 6,
                    Id_Entidad = 7,
                    Id_Tipo = 1,
                    Fecha_Alta = new DateTime(2021, 7, 22),
                    Estado = "ACTIVO"
                },
                new EntidadTipo
                {
                    Id_EntidadTipo = 7,
                    Id_Entidad = 8,
                    Id_Tipo = 1,
                    Fecha_Alta = new DateTime(2022, 11, 5),
                    Estado = "INACTIVO"
                },
                new EntidadTipo
                {
                    Id_EntidadTipo = 8,
                    Id_Entidad = 9,
                    Id_Tipo = 1,
                    Fecha_Alta = new DateTime(2023, 1, 10),
                    Estado = "ACTIVO"
                },
                new EntidadTipo
                {
                    Id_EntidadTipo = 9,
                    Id_Entidad = 10,
                    Id_Tipo = 1,
                    Fecha_Alta = new DateTime(2024, 6, 18),
                    Estado = "ACTIVO"
                },
                new EntidadTipo
                {
                    Id_EntidadTipo = 10,
                    Id_Entidad = 11,
                    Id_Tipo = 1,
                    Fecha_Alta = new DateTime(2020, 9, 30),
                    Estado = "ACTIVO"
                },
                new EntidadTipo
                {
                    Id_EntidadTipo = 11,
                    Id_Entidad = 12,
                    Id_Tipo = 1,
                    Fecha_Alta = new DateTime(2021, 12, 25),
                    Estado = "INACTIVO"
                },
                new EntidadTipo
                {
                    Id_EntidadTipo = 12,
                    Id_Entidad = 13,
                    Id_Tipo = 1,
                    Fecha_Alta = new DateTime(2022, 4, 8),
                    Estado = "ACTIVO"
                },
                new EntidadTipo
                {
                    Id_EntidadTipo = 13,
                    Id_Entidad = 14,
                    Id_Tipo = 1,
                    Fecha_Alta = new DateTime(2025, 2, 14),
                    Estado = "ACTIVO"
                }
            );

            // Seed de EntidadBajas de prueba
            modelBuilder.Entity<EntidadBaja>().HasData(
                new EntidadBaja 
                { 
                    Id_Baja = 1, 
                    Id_EntidadTipo = 1, 
                    Fecha_Baja = new DateTime(2016,1,18, 0, 0, 0), 
                    Motivo = "MORA" 
                },
                new EntidadBaja 
                { 
                    Id_Baja = 2, 
                    Id_EntidadTipo = 3, 
                    Fecha_Baja = new DateTime(2015,12,11, 0, 0, 0), 
                    Motivo = "RENUNCIA" 
                }
            );

            // Seed de Colaboradores de prueba
            modelBuilder.Entity<Colaborador>().HasData(
                new Colaborador 
                { 
                    Id_Colaborador = 1, 
                    Id_Prestacion = 2, 
                    Id_Entidad = 4 
                }
            );

            // Seed de Contactos de prueba
            modelBuilder.Entity<Contacto>().HasData(
                new Contacto 
                { 
                    Id_Contacto = 1, 
                    Tipo = "Celular", 
                    ContactoEntidad = "3413458966", 
                    Id_Entidad = 1 
                },
                new Contacto 
                { 
                    Id_Contacto = 2, 
                    Tipo = "Mail", 
                    ContactoEntidad = "luciano@gmail", 
                    Id_Entidad = 1 
                },
                new Contacto
                {
                    Id_Contacto = 3,
                    Tipo = "Celular",
                    ContactoEntidad = "3413485977",
                    Id_Entidad = 4
                },
                new Contacto
                {
                    Id_Contacto = 4,
                    Tipo = "Emergencia",
                    ContactoEntidad = "08007775427",
                    Id_Entidad = 3
                }
            );
            // Seed de Nicho de prueba
            modelBuilder.Entity<Nicho>().HasData(
                new Nicho
                {
                    Id_Nicho = 1,
                    Id_Entidad = 1,
                    Sector = "A",
                    NroNicho = "8",
                    ValorNicho = 2500000,
                    ValorLapida = 50000,
                    ConLapida = "SI",
                    Ocupado = "SI",
                    Cuotas = 2,
                    InteresMensual = 2.5m
                },
                new Nicho
                {
                    Id_Nicho = 2,
                    Id_Entidad = null,
                    Sector = "B",
                    NroNicho = "1",
                    ValorNicho = 2500000,
                    ValorLapida = null,
                    ConLapida = "NO",
                    Ocupado = "NO",
                    Cuotas = null,
                    InteresMensual = null
                }
                );

            // Seed de Tipo de Plan de prueba
            modelBuilder.Entity<TipoPlan>().HasData(
                new TipoPlan
                {
                    Id_TipoPlan = 1,
                    Id_TipoCuota = 2,
                    EdadTope = 65,
                    Tipo_Plan = "A",
                },
                new TipoPlan
                {
                    Id_TipoPlan = 2,
                    Id_TipoCuota = 3,
                    EdadTope = 65,
                    Tipo_Plan = "B",
                },
                new TipoPlan
                {
                    Id_TipoPlan = 3,
                    Id_TipoCuota = 4,
                    EdadTope = null,
                    Tipo_Plan = "A",
                },
                new TipoPlan
                {
                    Id_TipoPlan = 4,
                    Id_TipoCuota = 5,
                    EdadTope = null,
                    Tipo_Plan = "B",
                }
            );
            // Seed de Tipo de Cuota de prueba
            modelBuilder.Entity<TipoCuota>().HasData(
                new TipoCuota
                {
                    Id_TipoCuota = 1,
                    Concepto = "SOCIO",
                    Importe = 4500,
                    Tiene_EdadTope = true,
                    Fecha_ultimamodif = new DateTime(2026, 3, 10, 0, 0, 0)
                },
                new TipoCuota
                {
                    Id_TipoCuota = 2,
                    Concepto = "SEPELIO",
                    Importe = 10000,
                    Tiene_EdadTope = true,
                    Fecha_ultimamodif = new DateTime(2026, 3, 10, 0, 0, 0)
                },
                new TipoCuota
                {
                    Id_TipoCuota = 3,
                    Concepto = "SEPELIO",
                    Importe = 12000,
                    Tiene_EdadTope = true,
                    Fecha_ultimamodif = new DateTime(2026, 3, 10, 0, 0, 0)
                },
                new TipoCuota
                {
                    Id_TipoCuota = 4,
                    Concepto = "SEPELIO",
                    Importe = 12000,
                    Tiene_EdadTope = false,
                    Fecha_ultimamodif = new DateTime(2026, 3, 10, 0, 0, 0)
                },
                new TipoCuota
                {
                    Id_TipoCuota = 5,
                    Concepto = "SEPELIO",
                    Importe = 14000,
                    Tiene_EdadTope = false,
                    Fecha_ultimamodif = new DateTime(2026, 3, 10, 0, 0, 0)
                },
                new TipoCuota
                {
                    Id_TipoCuota = 6,
                    Concepto = "NICHO",
                    Importe = 300000,
                    Tiene_EdadTope = false,
                    Fecha_ultimamodif = new DateTime(2026, 3, 10, 0, 0, 0)
                }
            );

            // Seed de Cierres Contables de prueba
            modelBuilder.Entity<CierreContable>().HasData(
                new CierreContable
                {
                    Id_Cierre = 1,
                    Tipo = "Mensual",
                    Mes_Periodo = 3,
                    Anio_Periodo = 2026,
                    Fechor = new DateTime(2026, 3, 31, 9, 0, 0),
                    Id_Usuario = 3
                },
                new CierreContable
                {
                    Id_Cierre = 2,
                    Tipo = "Anual",
                    Mes_Periodo = null,
                    Anio_Periodo = 2025,
                    Fechor = new DateTime(2026, 1, 10, 8, 30, 0),
                    Id_Usuario = 2
                }
            );

            // Seed de MovimientosPrestacion de prueba
            modelBuilder.Entity<MovimientoPrestacion>().HasData(
                new MovimientoPrestacion
                {
                    Id_MovimientoPrestacion = 1,
                    Id_Movimiento = 3,
                    Id_Prestacion = 1
                }
            );

            // Seed de Cuotas de prueba
            modelBuilder.Entity<Cuota>().HasData(
                new Cuota
                {
                    Id_Deuda = 1,
                    Id_Entidad = 1,
                    Mes_Periodo = 6,
                    Anio_Periodo = 2026,
                    Id_TipoCuota = 1,
                    Monto = 4500,
                    FechaHoraGeneracion = new DateTime(2026, 6, 10, 8, 20, 0),
                    FechaVencimiento = new DateTime(2026, 6, 30, 0, 0, 0),
                    Estado = "PAGADO"
                },
                new Cuota
                {
                    Id_Deuda = 2,
                    Id_Entidad = 1,
                    Mes_Periodo = 7,
                    Anio_Periodo = 2026,
                    Id_TipoCuota = 1,
                    Monto = 4500,
                    FechaHoraGeneracion = new DateTime(2026, 6, 27, 8, 20, 0),
                    FechaVencimiento = new DateTime(2026, 7, 30, 0, 0, 0),
                    Estado = "PENDIENTE"
                }
            );
        }
    }
}
