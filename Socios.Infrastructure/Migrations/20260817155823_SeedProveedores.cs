using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedProveedores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "entidades",
                columns: new[] { "Id_Entidad", "Altura", "Apellido", "Calle", "CuitCuil", "Dni", "Id_Ciudad", "Nacimiento", "Nombre", "Observacion", "RazonSocial", "Sexo", "Tipo" },
                values: new object[,]
                {
                    { 1001, 1200, null, "San Martín", "30711111118", null, 3, null, null, null, "Distribuidora del Litoral S.A.", "Persona Juridica", "CUIT" },
                    { 1002, 850, null, "Córdoba", "30722222229", null, 3, null, null, null, "Insumos Médicos Rosario S.R.L.", "Persona Juridica", "CUIT" },
                    { 1003, 340, null, "Belgrano", null, "27333444", 2, null, null, null, "Servicios Integrales Funes", null, "DNI" },
                    { 1004, 55, null, "Sarmiento", "30744444441", null, 1, null, null, null, "Mantenimiento Roldán S.A.", "Persona Juridica", "CUIT" }
                });

            migrationBuilder.InsertData(
                table: "contactos",
                columns: new[] { "Id_Contacto", "ContactoEntidad", "Id_Entidad", "Tipo" },
                values: new object[,]
                {
                    { 1001, "3415550001", 1001, "TELEFONO" },
                    { 1002, "ventas@litoral.com", 1001, "MAIL" },
                    { 1003, "3415550002", 1002, "TELEFONO" },
                    { 1004, "3415550003", 1003, "TELEFONO" },
                    { 1005, "3415550004", 1004, "TELEFONO" }
                });

            migrationBuilder.InsertData(
                table: "entidad_tipos",
                columns: new[] { "Id_EntidadTipo", "Estado", "Fecha_Alta", "Id_Entidad", "Id_Tipo" },
                values: new object[,]
                {
                    { 1001, "ACTIVO", new DateTime(2023, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1001, 2 },
                    { 1002, "ACTIVO", new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1002, 2 },
                    { 1003, "ACTIVO", new DateTime(2022, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1003, 2 },
                    { 1004, "INACTIVO", new DateTime(2021, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1004, 2 }
                });

            migrationBuilder.InsertData(
                table: "proveedores",
                columns: new[] { "Id_Proveedor", "Id_Entidad", "Id_Prestacion" },
                values: new object[,]
                {
                    { 1001, 1001, 1 },
                    { 1002, 1002, 2 },
                    { 1003, 1003, 3 },
                    { 1004, 1004, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "proveedores",
                keyColumn: "Id_Proveedor",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "proveedores",
                keyColumn: "Id_Proveedor",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "proveedores",
                keyColumn: "Id_Proveedor",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "proveedores",
                keyColumn: "Id_Proveedor",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 1004);
        }
    }
}
