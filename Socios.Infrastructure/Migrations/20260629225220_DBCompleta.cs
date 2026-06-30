using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DBCompleta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cierres_contables",
                columns: table => new
                {
                    Id_Cierre = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Mes_Periodo = table.Column<int>(type: "int", nullable: true),
                    Anio_Periodo = table.Column<int>(type: "int", nullable: true),
                    Fechor = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Id_Usuario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cierres_contables", x => x.Id_Cierre);
                    table.ForeignKey(
                        name: "FK_cierres_contables_usuarios_Id_Usuario",
                        column: x => x.Id_Usuario,
                        principalTable: "usuarios",
                        principalColumn: "Id_Usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ciudades",
                columns: table => new
                {
                    Id_Ciudad = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ciudades", x => x.Id_Ciudad);
                });

            migrationBuilder.CreateTable(
                name: "cuentas",
                columns: table => new
                {
                    Id_Cuenta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreCuenta = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NroCuenta = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuentas", x => x.Id_Cuenta);
                });

            migrationBuilder.CreateTable(
                name: "detalle_movimientos",
                columns: table => new
                {
                    Id_Detmov = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreDetalleMovimiento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_movimientos", x => x.Id_Detmov);
                });

            migrationBuilder.CreateTable(
                name: "estado_cajadiaria",
                columns: table => new
                {
                    Id_CajaDiaria = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nro_Caja = table.Column<int>(type: "integer", nullable: false),
                    Id_Usuario = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Saldo = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estado_cajadiaria", x => x.Id_CajaDiaria);
                    table.ForeignKey(
                        name: "FK_estado_cajadiaria_usuarios_Id_Usuario",
                        column: x => x.Id_Usuario,
                        principalTable: "usuarios",
                        principalColumn: "Id_Usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metodos_pago",
                columns: table => new
                {
                    Id_MetodoPago = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreMetodoPago = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metodos_pago", x => x.Id_MetodoPago);
                });

            migrationBuilder.CreateTable(
                name: "obras_sociales",
                columns: table => new
                {
                    Id_ObraSocial = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreObraSocial = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_obras_sociales", x => x.Id_ObraSocial);
                });

            migrationBuilder.CreateTable(
                name: "prestaciones",
                columns: table => new
                {
                    Id_Prestacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombrePrestacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prestaciones", x => x.Id_Prestacion);
                });

            migrationBuilder.CreateTable(
                name: "tipo_cuotas",
                columns: table => new
                {
                    Id_TipoCuota = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Concepto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Importe = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Tiene_EdadTope = table.Column<bool>(type: "boolean", nullable: false),
                    Fecha_ultimamodif = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_cuotas", x => x.Id_TipoCuota);
                });

            migrationBuilder.CreateTable(
                name: "tipo_entidades",
                columns: table => new
                {
                    Id_Tipo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreTipoEntidad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_entidades", x => x.Id_Tipo);
                });

            migrationBuilder.CreateTable(
                name: "entidades",
                columns: table => new
                {
                    Id_Entidad = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Dni = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CuitCuil = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Apellido = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    RazonSocial = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Sexo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Nacimiento = table.Column<DateTime>(type: "date", nullable: true),
                    Id_Ciudad = table.Column<int>(type: "integer", nullable: false),
                    Calle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Altura = table.Column<int>(type: "integer", nullable: true),
                    Observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entidades", x => x.Id_Entidad);
                    table.ForeignKey(
                        name: "FK_entidades_ciudades_Id_Ciudad",
                        column: x => x.Id_Ciudad,
                        principalTable: "ciudades",
                        principalColumn: "Id_Ciudad",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tipo_planes",
                columns: table => new
                {
                    Id_TipoPlan = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_TipoCuota = table.Column<int>(type: "integer", nullable: false),
                    EdadTope = table.Column<int>(type: "integer", nullable: true),
                    Tipo_Plan = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_planes", x => x.Id_TipoPlan);
                    table.ForeignKey(
                        name: "FK_tipo_planes_tipo_cuotas_Id_TipoCuota",
                        column: x => x.Id_TipoCuota,
                        principalTable: "tipo_cuotas",
                        principalColumn: "Id_TipoCuota",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "caja",
                columns: table => new
                {
                    Id_Movimiento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_CajaDiaria = table.Column<int>(type: "integer", nullable: false),
                    FechaHoraMov = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Tipo_Movimiento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Id_Entidad = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Id_MetodoPago = table.Column<int>(type: "integer", nullable: false),
                    Id_Cuenta = table.Column<int>(type: "integer", nullable: false),
                    Id_Detmov = table.Column<int>(type: "integer", nullable: false),
                    Observacion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_caja", x => x.Id_Movimiento);
                    table.ForeignKey(
                        name: "FK_caja_cuentas_Id_Cuenta",
                        column: x => x.Id_Cuenta,
                        principalTable: "cuentas",
                        principalColumn: "Id_Cuenta",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_caja_detalle_movimientos_Id_Detmov",
                        column: x => x.Id_Detmov,
                        principalTable: "detalle_movimientos",
                        principalColumn: "Id_Detmov",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_caja_entidades_Id_Entidad",
                        column: x => x.Id_Entidad,
                        principalTable: "entidades",
                        principalColumn: "Id_Entidad",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_caja_estado_cajadiaria_Id_CajaDiaria",
                        column: x => x.Id_CajaDiaria,
                        principalTable: "estado_cajadiaria",
                        principalColumn: "Id_CajaDiaria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_caja_metodos_pago_Id_MetodoPago",
                        column: x => x.Id_MetodoPago,
                        principalTable: "metodos_pago",
                        principalColumn: "Id_MetodoPago",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "codeudores",
                columns: table => new
                {
                    Id_Codeudor = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_EntidadCodeudor = table.Column<int>(type: "integer", nullable: false),
                    Id_Entidad = table.Column<int>(type: "integer", nullable: false),
                    EntidadCodeudorId_Entidad = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_codeudores", x => x.Id_Codeudor);
                    table.ForeignKey(
                        name: "FK_codeudores_entidades_EntidadCodeudorId_Entidad",
                        column: x => x.EntidadCodeudorId_Entidad,
                        principalTable: "entidades",
                        principalColumn: "Id_Entidad");
                    table.ForeignKey(
                        name: "FK_codeudores_entidades_Id_EntidadCodeudor",
                        column: x => x.Id_EntidadCodeudor,
                        principalTable: "entidades",
                        principalColumn: "Id_Entidad",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "colaboradores",
                columns: table => new
                {
                    Id_Colaborador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Prestacion = table.Column<int>(type: "integer", nullable: false),
                    Id_Entidad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_colaboradores", x => x.Id_Colaborador);
                    table.ForeignKey(
                        name: "FK_colaboradores_entidades_Id_Entidad",
                        column: x => x.Id_Entidad,
                        principalTable: "entidades",
                        principalColumn: "Id_Entidad",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_colaboradores_prestaciones_Id_Prestacion",
                        column: x => x.Id_Prestacion,
                        principalTable: "prestaciones",
                        principalColumn: "Id_Prestacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contactos",
                columns: table => new
                {
                    Id_Contacto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContactoEntidad = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Id_Entidad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contactos", x => x.Id_Contacto);
                    table.ForeignKey(
                        name: "FK_contactos_entidades_Id_Entidad",
                        column: x => x.Id_Entidad,
                        principalTable: "entidades",
                        principalColumn: "Id_Entidad",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "entidad_tipos",
                columns: table => new
                {
                    Id_EntidadTipo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Entidad = table.Column<int>(type: "integer", nullable: false),
                    Id_Tipo = table.Column<int>(type: "integer", nullable: false),
                    Fecha_Alta = table.Column<DateTime>(type: "date", nullable: false),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entidad_tipos", x => x.Id_EntidadTipo);
                    table.ForeignKey(
                        name: "FK_entidad_tipos_entidades_Id_Entidad",
                        column: x => x.Id_Entidad,
                        principalTable: "entidades",
                        principalColumn: "Id_Entidad",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_entidad_tipos_tipo_entidades_Id_Tipo",
                        column: x => x.Id_Tipo,
                        principalTable: "tipo_entidades",
                        principalColumn: "Id_Tipo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "nichos",
                columns: table => new
                {
                    Id_Nicho = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Entidad = table.Column<int>(type: "integer", nullable: true),
                    Sector = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NroNicho = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ValorNicho = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorLapida = table.Column<decimal>(type: "numeric", nullable: true),
                    ConLapida = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Ocupado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Cuotas = table.Column<int>(type: "int", nullable: true),
                    InteresMensual = table.Column<decimal>(type: "numeric(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nichos", x => x.Id_Nicho);
                    table.ForeignKey(
                        name: "FK_nichos_entidades_Id_Entidad",
                        column: x => x.Id_Entidad,
                        principalTable: "entidades",
                        principalColumn: "Id_Entidad",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "socios",
                columns: table => new
                {
                    Id_Socio = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Entidad = table.Column<int>(type: "integer", nullable: false),
                    Id_OS = table.Column<int>(type: "integer", nullable: false),
                    Plan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Sepelio = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Cobrador = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Numero_Afiliado = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_socios", x => x.Id_Socio);
                    table.ForeignKey(
                        name: "FK_socios_entidades_Id_Entidad",
                        column: x => x.Id_Entidad,
                        principalTable: "entidades",
                        principalColumn: "Id_Entidad",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_socios_obras_sociales_Id_OS",
                        column: x => x.Id_OS,
                        principalTable: "obras_sociales",
                        principalColumn: "Id_ObraSocial",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cuotas",
                columns: table => new
                {
                    Id_Deuda = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Entidad = table.Column<int>(type: "integer", nullable: false),
                    Mes_Periodo = table.Column<int>(type: "int", nullable: false),
                    Anio_Periodo = table.Column<int>(type: "int", nullable: false),
                    Id_TipoCuota = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaHoraGeneracion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "date", nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Id_Movimiento = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuotas", x => x.Id_Deuda);
                    table.ForeignKey(
                        name: "FK_cuotas_caja_Id_Movimiento",
                        column: x => x.Id_Movimiento,
                        principalTable: "caja",
                        principalColumn: "Id_Movimiento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cuotas_entidades_Id_Entidad",
                        column: x => x.Id_Entidad,
                        principalTable: "entidades",
                        principalColumn: "Id_Entidad",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cuotas_tipo_cuotas_Id_TipoCuota",
                        column: x => x.Id_TipoCuota,
                        principalTable: "tipo_cuotas",
                        principalColumn: "Id_TipoCuota",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movimientos_prestaciones",
                columns: table => new
                {
                    Id_MovimientoPrestacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Movimiento = table.Column<int>(type: "integer", nullable: false),
                    Id_Prestacion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimientos_prestaciones", x => x.Id_MovimientoPrestacion);
                    table.ForeignKey(
                        name: "FK_movimientos_prestaciones_caja_Id_Movimiento",
                        column: x => x.Id_Movimiento,
                        principalTable: "caja",
                        principalColumn: "Id_Movimiento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movimientos_prestaciones_prestaciones_Id_Prestacion",
                        column: x => x.Id_Prestacion,
                        principalTable: "prestaciones",
                        principalColumn: "Id_Prestacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "entidad_bajas",
                columns: table => new
                {
                    Id_Baja = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_EntidadTipo = table.Column<int>(type: "integer", nullable: false),
                    Fecha_Baja = table.Column<DateTime>(type: "date", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entidad_bajas", x => x.Id_Baja);
                    table.ForeignKey(
                        name: "FK_entidad_bajas_entidad_tipos_Id_EntidadTipo",
                        column: x => x.Id_EntidadTipo,
                        principalTable: "entidad_tipos",
                        principalColumn: "Id_EntidadTipo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ciudades",
                columns: new[] { "Id_Ciudad", "Nombre" },
                values: new object[,]
                {
                    { 1, "Roldán" },
                    { 2, "Funes" },
                    { 3, "Rosario" }
                });

            migrationBuilder.InsertData(
                table: "cuentas",
                columns: new[] { "Id_Cuenta", "Estado", "NombreCuenta", "NroCuenta" },
                values: new object[,]
                {
                    { 1, "Activa", "Efectivo", 0 },
                    { 2, "Activa", "Banco Macro", 789012 },
                    { 3, "Inactiva", "Banco Macro 2", 712906 }
                });

            migrationBuilder.InsertData(
                table: "detalle_movimientos",
                columns: new[] { "Id_Detmov", "NombreDetalleMovimiento" },
                values: new object[,]
                {
                    { 1, "Cuota Socio" },
                    { 2, "Cuota Sepelio" },
                    { 3, "Ajuste Diferencia Caja" },
                    { 4, "Colaboración" },
                    { 5, "Pago a Proveedor" },
                    { 6, "Transferencia" }
                });

            migrationBuilder.InsertData(
                table: "estado_cajadiaria",
                columns: new[] { "Id_CajaDiaria", "FechaHora", "Id_Usuario", "Nro_Caja", "Saldo", "Tipo" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 9, 8, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 100000m, "Apertura" },
                    { 2, new DateTime(2026, 6, 9, 12, 5, 0, 0, DateTimeKind.Unspecified), 1, 1, 142000m, "Cierre" }
                });

            migrationBuilder.InsertData(
                table: "metodos_pago",
                columns: new[] { "Id_MetodoPago", "NombreMetodoPago" },
                values: new object[,]
                {
                    { 1, "Efectivo" },
                    { 2, "Transferencia Bancaria" }
                });

            migrationBuilder.InsertData(
                table: "nichos",
                columns: new[] { "Id_Nicho", "ConLapida", "Cuotas", "Id_Entidad", "InteresMensual", "NroNicho", "Ocupado", "Sector", "ValorLapida", "ValorNicho" },
                values: new object[] { 2, "NO", null, null, null, "1", "NO", "B", null, 2500000m });

            migrationBuilder.InsertData(
                table: "obras_sociales",
                columns: new[] { "Id_ObraSocial", "NombreObraSocial" },
                values: new object[,]
                {
                    { 1, "PAMI" },
                    { 2, "OSDE" }
                });

            migrationBuilder.InsertData(
                table: "prestaciones",
                columns: new[] { "Id_Prestacion", "NombrePrestacion" },
                values: new object[,]
                {
                    { 1, "Podología" },
                    { 2, "Cardiología" },
                    { 3, "Alquiler Salón" },
                    { 4, "Comisión Cobrador" }
                });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id_Rol",
                keyValue: 1,
                column: "Descripcion",
                value: "Secretaria que maneja todo el sistema");

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id_Rol", "Descripcion", "RolNombre" },
                values: new object[] { 2, "Solo posee acceso al módulo de informes", "Consultor" });

            migrationBuilder.InsertData(
                table: "tipo_cuotas",
                columns: new[] { "Id_TipoCuota", "Concepto", "Fecha_ultimamodif", "Importe", "Tiene_EdadTope" },
                values: new object[,]
                {
                    { 1, "SOCIO", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 4500m, true },
                    { 2, "SEPELIO", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 10000m, true },
                    { 3, "SEPELIO", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 12000m, true },
                    { 4, "SEPELIO", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 12000m, false },
                    { 5, "SEPELIO", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 14000m, false },
                    { 6, "NICHO", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 300000m, false }
                });

            migrationBuilder.InsertData(
                table: "tipo_entidades",
                columns: new[] { "Id_Tipo", "NombreTipoEntidad" },
                values: new object[,]
                {
                    { 1, "Socio" },
                    { 2, "Proveedor" },
                    { 3, "Colaborador" },
                    { 4, "Empleado" }
                });

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 1,
                column: "Id_Rol",
                value: 2);

            migrationBuilder.InsertData(
                table: "usuarios",
                columns: new[] { "Id_Usuario", "Estado", "Id_Rol", "Password", "UsuarioNombre" },
                values: new object[,]
                {
                    { 2, "Activo", 1, "$2a$11$5.O9NB.FFBZ98GrE24jq7et8c0ACOkRVsSueihm78or/JNZmjVGay", "Adriana" },
                    { 3, "Activo", 1, "$2a$11$5.O9NB.FFBZ98GrE24jq7et8c0ACOkRVsSueihm78or/JNZmjVGay", "Marcela" }
                });

            migrationBuilder.InsertData(
                table: "cierres_contables",
                columns: new[] { "Id_Cierre", "Anio_Periodo", "Fechor", "Id_Usuario", "Mes_Periodo", "Tipo" },
                values: new object[,]
                {
                    { 1, 2026, new DateTime(2026, 3, 31, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, 3, "Mensual" },
                    { 2, 2025, new DateTime(2026, 1, 10, 8, 30, 0, 0, DateTimeKind.Unspecified), 2, null, "Anual" }
                });

            migrationBuilder.InsertData(
                table: "entidades",
                columns: new[] { "Id_Entidad", "Altura", "Apellido", "Calle", "CuitCuil", "Dni", "Id_Ciudad", "Nacimiento", "Nombre", "Observacion", "RazonSocial", "Sexo", "Tipo" },
                values: new object[,]
                {
                    { 1, 250, "Oldan", "Independencia", "20321270574", "32127057", 1, new DateTime(1959, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Luciano", null, null, "Hombre", "DNI" },
                    { 2, 458, "Pérez", "Galindo", null, "32185166", 2, new DateTime(1998, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Juan", null, null, "Hombre", "DNI" },
                    { 3, 166, null, "Mitre", "30657866330", null, 3, new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Litoral Gas S.A", "Persona Juridica", "DNI" },
                    { 4, 347, "López", "Las Heras", null, "55406681", 1, new DateTime(1987, 12, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "María", null, null, "Mujer", "DNI" }
                });

            migrationBuilder.InsertData(
                table: "estado_cajadiaria",
                columns: new[] { "Id_CajaDiaria", "FechaHora", "Id_Usuario", "Nro_Caja", "Saldo", "Tipo" },
                values: new object[,]
                {
                    { 3, new DateTime(2026, 6, 9, 8, 10, 0, 0, DateTimeKind.Unspecified), 2, 2, 78000m, "Apertura" },
                    { 4, new DateTime(2026, 6, 9, 12, 15, 0, 0, DateTimeKind.Unspecified), 2, 2, 250000m, "Cierre" }
                });

            migrationBuilder.InsertData(
                table: "tipo_planes",
                columns: new[] { "Id_TipoPlan", "EdadTope", "Id_TipoCuota", "Tipo_Plan" },
                values: new object[,]
                {
                    { 1, 65, 2, "A" },
                    { 2, 65, 3, "B" },
                    { 3, null, 4, "A" },
                    { 4, null, 5, "B" }
                });

            migrationBuilder.InsertData(
                table: "caja",
                columns: new[] { "Id_Movimiento", "FechaHoraMov", "Id_CajaDiaria", "Id_Cuenta", "Id_Detmov", "Id_Entidad", "Id_MetodoPago", "Monto", "Observacion", "Tipo_Movimiento" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 10, 8, 20, 0, 0, DateTimeKind.Unspecified), 1, 1, 1, 1, 2, 13500m, "Pago Cuota Mayo", "Ingreso" },
                    { 2, new DateTime(2026, 6, 10, 9, 30, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 3, 1, 42500.47m, "Pago Gas Mes Junio", "Egreso" },
                    { 3, new DateTime(2026, 6, 10, 9, 30, 0, 0, DateTimeKind.Unspecified), 2, 2, 4, 4, 1, 30000m, "Colaboración Podología", "Ingreso" }
                });

            migrationBuilder.InsertData(
                table: "codeudores",
                columns: new[] { "Id_Codeudor", "EntidadCodeudorId_Entidad", "Id_Entidad", "Id_EntidadCodeudor" },
                values: new object[] { 1, null, 2, 1 });

            migrationBuilder.InsertData(
                table: "colaboradores",
                columns: new[] { "Id_Colaborador", "Id_Entidad", "Id_Prestacion" },
                values: new object[] { 1, 4, 2 });

            migrationBuilder.InsertData(
                table: "contactos",
                columns: new[] { "Id_Contacto", "ContactoEntidad", "Id_Entidad", "Tipo" },
                values: new object[,]
                {
                    { 1, "3413458966", 1, "Celular" },
                    { 2, "luciano@gmail", 1, "Mail" },
                    { 3, "3413485977", 4, "Celular" },
                    { 4, "08007775427", 3, "Emergencia" }
                });

            migrationBuilder.InsertData(
                table: "cuotas",
                columns: new[] { "Id_Deuda", "Anio_Periodo", "Estado", "FechaHoraGeneracion", "FechaVencimiento", "Id_Entidad", "Id_Movimiento", "Id_TipoCuota", "Mes_Periodo", "Monto" },
                values: new object[,]
                {
                    { 1, 2026, "PAGADO", new DateTime(2026, 6, 10, 8, 20, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 1, 6, 4500m },
                    { 2, 2026, "PENDIENTE", new DateTime(2026, 6, 27, 8, 20, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 1, 7, 4500m }
                });

            migrationBuilder.InsertData(
                table: "entidad_tipos",
                columns: new[] { "Id_EntidadTipo", "Estado", "Fecha_Alta", "Id_Entidad", "Id_Tipo" },
                values: new object[,]
                {
                    { 1, "INACTIVO", new DateTime(1995, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, "ACTIVO", new DateTime(2000, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3 },
                    { 3, "INACTIVO", new DateTime(2008, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 3 }
                });

            migrationBuilder.InsertData(
                table: "nichos",
                columns: new[] { "Id_Nicho", "ConLapida", "Cuotas", "Id_Entidad", "InteresMensual", "NroNicho", "Ocupado", "Sector", "ValorLapida", "ValorNicho" },
                values: new object[] { 1, "SI", 2, 1, 2.5m, "8", "SI", "A", 50000m, 2500000m });

            migrationBuilder.InsertData(
                table: "socios",
                columns: new[] { "Id_Socio", "Cobrador", "Id_Entidad", "Id_OS", "Numero_Afiliado", "Plan", "Sepelio" },
                values: new object[] { 1, "NO", 1, 1, "", "A", "SI" });

            migrationBuilder.InsertData(
                table: "entidad_bajas",
                columns: new[] { "Id_Baja", "Fecha_Baja", "Id_EntidadTipo", "Motivo" },
                values: new object[,]
                {
                    { 1, new DateTime(2016, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "MORA" },
                    { 2, new DateTime(2015, 12, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "RENUNCIA" }
                });

            migrationBuilder.InsertData(
                table: "movimientos_prestaciones",
                columns: new[] { "Id_MovimientoPrestacion", "Id_Movimiento", "Id_Prestacion" },
                values: new object[] { 1, 3, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_caja_Id_CajaDiaria",
                table: "caja",
                column: "Id_CajaDiaria");

            migrationBuilder.CreateIndex(
                name: "IX_caja_Id_Cuenta",
                table: "caja",
                column: "Id_Cuenta");

            migrationBuilder.CreateIndex(
                name: "IX_caja_Id_Detmov",
                table: "caja",
                column: "Id_Detmov");

            migrationBuilder.CreateIndex(
                name: "IX_caja_Id_Entidad",
                table: "caja",
                column: "Id_Entidad");

            migrationBuilder.CreateIndex(
                name: "IX_caja_Id_MetodoPago",
                table: "caja",
                column: "Id_MetodoPago");

            migrationBuilder.CreateIndex(
                name: "IX_cierres_contables_Id_Usuario",
                table: "cierres_contables",
                column: "Id_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_codeudores_EntidadCodeudorId_Entidad",
                table: "codeudores",
                column: "EntidadCodeudorId_Entidad");

            migrationBuilder.CreateIndex(
                name: "IX_codeudores_Id_EntidadCodeudor",
                table: "codeudores",
                column: "Id_EntidadCodeudor");

            migrationBuilder.CreateIndex(
                name: "IX_colaboradores_Id_Entidad",
                table: "colaboradores",
                column: "Id_Entidad");

            migrationBuilder.CreateIndex(
                name: "IX_colaboradores_Id_Prestacion",
                table: "colaboradores",
                column: "Id_Prestacion");

            migrationBuilder.CreateIndex(
                name: "IX_contactos_Id_Entidad",
                table: "contactos",
                column: "Id_Entidad");

            migrationBuilder.CreateIndex(
                name: "IX_cuotas_Id_Entidad",
                table: "cuotas",
                column: "Id_Entidad");

            migrationBuilder.CreateIndex(
                name: "IX_cuotas_Id_Movimiento",
                table: "cuotas",
                column: "Id_Movimiento");

            migrationBuilder.CreateIndex(
                name: "IX_cuotas_Id_TipoCuota",
                table: "cuotas",
                column: "Id_TipoCuota");

            migrationBuilder.CreateIndex(
                name: "IX_entidad_bajas_Id_EntidadTipo",
                table: "entidad_bajas",
                column: "Id_EntidadTipo");

            migrationBuilder.CreateIndex(
                name: "IX_entidad_tipos_Id_Entidad",
                table: "entidad_tipos",
                column: "Id_Entidad");

            migrationBuilder.CreateIndex(
                name: "IX_entidad_tipos_Id_Tipo",
                table: "entidad_tipos",
                column: "Id_Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_entidades_Id_Ciudad",
                table: "entidades",
                column: "Id_Ciudad");

            migrationBuilder.CreateIndex(
                name: "IX_estado_cajadiaria_Id_Usuario",
                table: "estado_cajadiaria",
                column: "Id_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_prestaciones_Id_Movimiento",
                table: "movimientos_prestaciones",
                column: "Id_Movimiento");

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_prestaciones_Id_Prestacion",
                table: "movimientos_prestaciones",
                column: "Id_Prestacion");

            migrationBuilder.CreateIndex(
                name: "IX_nichos_Id_Entidad",
                table: "nichos",
                column: "Id_Entidad");

            migrationBuilder.CreateIndex(
                name: "IX_socios_Id_Entidad",
                table: "socios",
                column: "Id_Entidad");

            migrationBuilder.CreateIndex(
                name: "IX_socios_Id_OS",
                table: "socios",
                column: "Id_OS");

            migrationBuilder.CreateIndex(
                name: "IX_tipo_planes_Id_TipoCuota",
                table: "tipo_planes",
                column: "Id_TipoCuota");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cierres_contables");

            migrationBuilder.DropTable(
                name: "codeudores");

            migrationBuilder.DropTable(
                name: "colaboradores");

            migrationBuilder.DropTable(
                name: "contactos");

            migrationBuilder.DropTable(
                name: "cuotas");

            migrationBuilder.DropTable(
                name: "entidad_bajas");

            migrationBuilder.DropTable(
                name: "movimientos_prestaciones");

            migrationBuilder.DropTable(
                name: "nichos");

            migrationBuilder.DropTable(
                name: "socios");

            migrationBuilder.DropTable(
                name: "tipo_planes");

            migrationBuilder.DropTable(
                name: "entidad_tipos");

            migrationBuilder.DropTable(
                name: "caja");

            migrationBuilder.DropTable(
                name: "prestaciones");

            migrationBuilder.DropTable(
                name: "obras_sociales");

            migrationBuilder.DropTable(
                name: "tipo_cuotas");

            migrationBuilder.DropTable(
                name: "tipo_entidades");

            migrationBuilder.DropTable(
                name: "cuentas");

            migrationBuilder.DropTable(
                name: "detalle_movimientos");

            migrationBuilder.DropTable(
                name: "entidades");

            migrationBuilder.DropTable(
                name: "estado_cajadiaria");

            migrationBuilder.DropTable(
                name: "metodos_pago");

            migrationBuilder.DropTable(
                name: "ciudades");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id_Rol",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id_Rol",
                keyValue: 1,
                column: "Descripcion",
                value: "secretaria que maneja todo el sistema");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 1,
                column: "Id_Rol",
                value: 1);
        }
    }
}
