using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class staging1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 4,
                column: "Tipo",
                value: "TELÉFONO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 4,
                column: "Tipo",
                value: "Emergencia");
        }
    }
}
