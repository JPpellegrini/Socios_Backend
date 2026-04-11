using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Usuario_Rol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "usuarios");

            migrationBuilder.AddColumn<int>(
                name: "Id_Usuario",
                table: "usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "usuarios",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Id_Rol",
                table: "usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNombre",
                table: "usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios",
                column: "Id_Usuario");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id_Rol = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RolNombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id_Rol);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id_Rol", "Descripcion", "RolNombre" },
                values: new object[] { 1, "secretaria que maneja todo el sistema", "Secretaria" });

            migrationBuilder.InsertData(
                table: "usuarios",
                columns: new[] { "Id_Usuario", "Estado", "Id_Rol", "Password", "UsuarioNombre" },
                values: new object[] { 1, "Activo", 1, "1234", "CJR" });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Id_Rol",
                table: "usuarios",
                column: "Id_Rol");

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_roles_Id_Rol",
                table: "usuarios",
                column: "Id_Rol",
                principalTable: "roles",
                principalColumn: "Id_Rol",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_roles_Id_Rol",
                table: "usuarios");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_Id_Rol",
                table: "usuarios");

            migrationBuilder.DeleteData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyColumnType: "integer",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Id_Usuario",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "Id_Rol",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioNombre",
                table: "usuarios");

            migrationBuilder.RenameTable(
                name: "usuarios",
                newName: "Usuarios");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Usuarios",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");
        }
    }
}
