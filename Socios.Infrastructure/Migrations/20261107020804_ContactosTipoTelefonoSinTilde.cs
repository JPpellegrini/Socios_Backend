using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ContactosTipoTelefonoSinTilde : Migration
    {
        /// <summary>
        /// Unifica el tipo de contacto "TELÉFONO" (con tilde) en "TELEFONO", que es el valor que
        /// escriben las altas y que buscan los listados e informes. Los contactos semilla 1, 3 y 4
        /// tenían tilde (la migración staging1 se lo volvió a poner al 4) y por eso esos teléfonos
        /// no aparecían en el informe de socios.
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 1,
                column: "Tipo",
                value: "TELEFONO");

            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 3,
                column: "Tipo",
                value: "TELEFONO");

            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 4,
                column: "Tipo",
                value: "TELEFONO");

            // Además, cualquier otro contacto que se haya cargado con tilde (fuera de la semilla).
            migrationBuilder.Sql("""UPDATE contactos SET "Tipo" = 'TELEFONO' WHERE "Tipo" = 'TELÉFONO';""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 1,
                column: "Tipo",
                value: "TELÉFONO");

            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 3,
                column: "Tipo",
                value: "TELÉFONO");

            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 4,
                column: "Tipo",
                value: "TELÉFONO");
        }
    }
}
