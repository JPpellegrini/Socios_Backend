using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregaSeedSocios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "socios",
                columns: new[] { "Id_Socio", "Cobrador", "Id_Entidad", "Id_OS", "Numero_Afiliado", "Plan", "Sepelio" },
                values: new object[,]
                {
                    { 4, "NO", 1, 1, "", "A", "SI" },
                    { 5, "SI", 1, 1, "", "B", "NO" },
                    { 6, "NO", 1, 1, "", "A", "NO" },
                    { 7, "SI", 1, 1, "", "B", "SI" },
                    { 8, "SI", 1, 1, "", "A", "SI" },
                    { 9, "NO", 1, 1, "", "B", "NO" },
                    { 10, "SI", 1, 1, "", "A", "NO" },
                    { 11, "NO", 1, 1, "", "B", "SI" },
                    { 12, "NO", 1, 1, "", "A", "SI" },
                    { 13, "SI", 1, 1, "", "B", "NO" },
                    { 14, "NO", 1, 1, "", "A", "NO" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 14);
        }
    }
}
