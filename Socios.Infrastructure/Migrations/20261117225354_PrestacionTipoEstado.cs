using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PrestacionTipoEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Las prestaciones ya cargadas quedan ACTIVAS y de tipo PRESTACION (revisar y corregir
            // el tipo de las que correspondan a COLABORACION). Sin estos valores por defecto, las
            // restricciones CHECK de abajo fallarían sobre las filas existentes.
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "prestaciones",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "ACTIVA");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "prestaciones",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "PRESTACION");

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 1,
                columns: new[] { "Estado", "Tipo" },
                values: new object[] { "ACTIVA", "PRESTACION" });

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 2,
                columns: new[] { "Estado", "Tipo" },
                values: new object[] { "ACTIVA", "PRESTACION" });

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 3,
                columns: new[] { "Estado", "Tipo" },
                values: new object[] { "ACTIVA", "PRESTACION" });

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 4,
                columns: new[] { "Estado", "Tipo" },
                values: new object[] { "ACTIVA", "PRESTACION" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_prestaciones_Estado",
                table: "prestaciones",
                sql: "\"Estado\" IN ('ACTIVA', 'INACTIVA')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_prestaciones_Tipo",
                table: "prestaciones",
                sql: "\"Tipo\" IN ('COLABORACION', 'PRESTACION')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_prestaciones_Estado",
                table: "prestaciones");

            migrationBuilder.DropCheckConstraint(
                name: "CK_prestaciones_Tipo",
                table: "prestaciones");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "prestaciones");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "prestaciones");
        }
    }
}
