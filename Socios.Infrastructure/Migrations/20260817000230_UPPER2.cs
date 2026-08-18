using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UPPER2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 1,
                column: "Estado",
                value: "ACTIVO");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 2,
                column: "Estado",
                value: "ACTIVO");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 3,
                column: "Estado",
                value: "INACTIVO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 1,
                column: "Estado",
                value: "Activo");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 2,
                column: "Estado",
                value: "Activo");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 3,
                column: "Estado",
                value: "Activo");
        }
    }
}
