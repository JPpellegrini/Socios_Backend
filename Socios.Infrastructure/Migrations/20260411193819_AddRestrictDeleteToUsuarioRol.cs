using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestrictDeleteToUsuarioRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_roles_Id_Rol",
                table: "usuarios");

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_roles_Id_Rol",
                table: "usuarios",
                column: "Id_Rol",
                principalTable: "roles",
                principalColumn: "Id_Rol",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_roles_Id_Rol",
                table: "usuarios");

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_roles_Id_Rol",
                table: "usuarios",
                column: "Id_Rol",
                principalTable: "roles",
                principalColumn: "Id_Rol",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
