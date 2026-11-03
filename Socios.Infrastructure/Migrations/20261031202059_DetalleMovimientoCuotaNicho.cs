using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DetalleMovimientoCuotaNicho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "detalle_movimientos",
                columns: new[] { "Id_Detmov", "NombreDetalleMovimiento" },
                values: new object[] { 7, "CUOTA NICHO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 7);
        }
    }
}
