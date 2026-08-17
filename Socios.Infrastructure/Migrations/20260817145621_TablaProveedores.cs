using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TablaProveedores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "proveedores",
                columns: table => new
                {
                    Id_Proveedor = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Prestacion = table.Column<int>(type: "integer", nullable: false),
                    Id_Entidad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proveedores", x => x.Id_Proveedor);
                    table.ForeignKey(
                        name: "FK_proveedores_entidades_Id_Entidad",
                        column: x => x.Id_Entidad,
                        principalTable: "entidades",
                        principalColumn: "Id_Entidad",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_proveedores_prestaciones_Id_Prestacion",
                        column: x => x.Id_Prestacion,
                        principalTable: "prestaciones",
                        principalColumn: "Id_Prestacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_proveedores_Id_Entidad",
                table: "proveedores",
                column: "Id_Entidad");

            migrationBuilder.CreateIndex(
                name: "IX_proveedores_Id_Prestacion",
                table: "proveedores",
                column: "Id_Prestacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "proveedores");
        }
    }
}
