using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UPPER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id_Rol",
                keyValue: 1,
                columns: new[] { "Descripcion", "RolNombre" },
                values: new object[] { "ADMINISTRATIVO QUE MANEJA TODO EL SISTEMA", "ADMINISTRATIVO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id_Rol",
                keyValue: 1,
                columns: new[] { "Descripcion", "RolNombre" },
                values: new object[] { "SECRETARIA QUE MANEJA TODO EL SISTEMA", "SECRETARIA" });
        }
    }
}
