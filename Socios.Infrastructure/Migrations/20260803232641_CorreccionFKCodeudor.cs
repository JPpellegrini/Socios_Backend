using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionFKCodeudor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_codeudores_entidades_EntidadCodeudorId_Entidad",
                table: "codeudores");

            migrationBuilder.DropIndex(
                name: "IX_codeudores_EntidadCodeudorId_Entidad",
                table: "codeudores");

            migrationBuilder.DropColumn(
                name: "EntidadCodeudorId_Entidad",
                table: "codeudores");

            migrationBuilder.CreateIndex(
                name: "IX_codeudores_Id_Entidad",
                table: "codeudores",
                column: "Id_Entidad");

            migrationBuilder.AddForeignKey(
                name: "FK_codeudores_entidades_Id_Entidad",
                table: "codeudores",
                column: "Id_Entidad",
                principalTable: "entidades",
                principalColumn: "Id_Entidad",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_codeudores_entidades_Id_Entidad",
                table: "codeudores");

            migrationBuilder.DropIndex(
                name: "IX_codeudores_Id_Entidad",
                table: "codeudores");

            migrationBuilder.AddColumn<int>(
                name: "EntidadCodeudorId_Entidad",
                table: "codeudores",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "codeudores",
                keyColumn: "Id_Codeudor",
                keyValue: 1,
                column: "EntidadCodeudorId_Entidad",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_codeudores_EntidadCodeudorId_Entidad",
                table: "codeudores",
                column: "EntidadCodeudorId_Entidad");

            migrationBuilder.AddForeignKey(
                name: "FK_codeudores_entidades_EntidadCodeudorId_Entidad",
                table: "codeudores",
                column: "EntidadCodeudorId_Entidad",
                principalTable: "entidades",
                principalColumn: "Id_Entidad");
        }
    }
}
