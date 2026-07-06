using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class nombre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "entidades",
                columns: new[] { "Id_Entidad", "Altura", "Apellido", "Calle", "CuitCuil", "Dni", "Id_Ciudad", "Nacimiento", "Nombre", "Observacion", "RazonSocial", "Sexo", "Tipo" },
                values: new object[,]
                {
                    { 5, 417, "AVILA", "Salta", null, "6166108", 1, new DateTime(1958, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "EDUARDO", null, null, "Hombre", "DNI" },
                    { 6, 270, "MASABEU", "Independencia", null, "6164804", 1, new DateTime(1960, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "RUTILIO ALFREDO", null, null, "Hombre", "DNI" },
                    { 7, 684, "MASABEU", "Mendoza", null, "14658096", 1, new DateTime(1955, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "ALFREDO", null, null, "Hombre", "DNI" },
                    { 8, 867, "MEYADO", "Corrientes", null, "4989850", 1, new DateTime(1962, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "JOSÉ MOISÉS", null, null, "Hombre", "DNI" },
                    { 9, 1044, "IPPOLITI", "Mendoza", null, "6176088", 1, new DateTime(1959, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "HUGO", null, null, "Hombre", "DNI" },
                    { 10, 417, "FLORIÁN DE AVILA", "Salta", null, "3247480", 1, new DateTime(1961, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "SUSANA", null, null, "Mujer", "DNI" },
                    { 11, 292, "HERRERA", "1 de Mayo", null, "3962438", 1, new DateTime(1956, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "LUCY", null, null, "Mujer", "DNI" },
                    { 12, 369, "MURATTURE", "Mitre", null, "4105793", 1, new DateTime(1957, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "IRIS A.", null, null, "Mujer", "DNI" },
                    { 13, 455, "VILLAREAL", "Jujuy", null, "12266301", 1, new DateTime(1963, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "RAQUEL", null, null, "Mujer", "DNI" },
                    { 14, 953, "BRAVO DE PONTI", "Jujuy", null, "4645560", 1, new DateTime(1964, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "ANA MARÍA", null, null, "Mujer", "DNI" }
                });

            migrationBuilder.InsertData(
                table: "entidad_tipos",
                columns: new[] { "Id_EntidadTipo", "Estado", "Fecha_Alta", "Id_Entidad", "Id_Tipo" },
                values: new object[,]
                {
                    { 4, "ACTIVO", new DateTime(1995, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1 },
                    { 5, "ACTIVO", new DateTime(2020, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 1 },
                    { 6, "ACTIVO", new DateTime(2021, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 1 },
                    { 7, "INACTIVO", new DateTime(2022, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 1 },
                    { 8, "ACTIVO", new DateTime(2023, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 1 },
                    { 9, "ACTIVO", new DateTime(2024, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 1 },
                    { 10, "ACTIVO", new DateTime(2020, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 1 },
                    { 11, "INACTIVO", new DateTime(2021, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, 1 },
                    { 12, "ACTIVO", new DateTime(2022, 4, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 13, 1 },
                    { 13, "ACTIVO", new DateTime(2025, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 14, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "entidad_tipos",
                keyColumn: "Id_EntidadTipo",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 14);
        }
    }
}
