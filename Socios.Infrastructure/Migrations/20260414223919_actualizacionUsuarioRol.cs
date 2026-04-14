using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class actualizacionUsuarioRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$wH6QnQnQnQnQnQnQnQnQOeQnQnQnQnQnQnQnQnQnQnQnQnQnQnQ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 1,
                column: "Password",
                value: "1234");
        }
    }
}
