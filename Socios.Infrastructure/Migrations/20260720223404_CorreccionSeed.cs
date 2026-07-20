using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 14);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 4,
                column: "Id_Entidad",
                value: 5);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 5,
                column: "Id_Entidad",
                value: 6);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 6,
                column: "Id_Entidad",
                value: 7);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 7,
                column: "Id_Entidad",
                value: 8);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 8,
                column: "Id_Entidad",
                value: 9);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 9,
                column: "Id_Entidad",
                value: 10);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 10,
                column: "Id_Entidad",
                value: 11);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 11,
                column: "Id_Entidad",
                value: 12);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 12,
                column: "Id_Entidad",
                value: 13);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 13,
                column: "Id_Entidad",
                value: 14);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 4,
                column: "Id_Entidad",
                value: 1);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 5,
                column: "Id_Entidad",
                value: 1);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 6,
                column: "Id_Entidad",
                value: 1);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 7,
                column: "Id_Entidad",
                value: 1);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 8,
                column: "Id_Entidad",
                value: 1);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 9,
                column: "Id_Entidad",
                value: 1);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 10,
                column: "Id_Entidad",
                value: 1);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 11,
                column: "Id_Entidad",
                value: 1);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 12,
                column: "Id_Entidad",
                value: 1);

            migrationBuilder.UpdateData(
                table: "socios",
                keyColumn: "Id_Socio",
                keyValue: 13,
                column: "Id_Entidad",
                value: 1);

            migrationBuilder.InsertData(
                table: "socios",
                columns: new[] { "Id_Socio", "Cobrador", "Id_Entidad", "Id_OS", "Numero_Afiliado", "Plan", "Sepelio" },
                values: new object[] { 14, "NO", 1, 1, "", "A", "NO" });
        }
    }
}
