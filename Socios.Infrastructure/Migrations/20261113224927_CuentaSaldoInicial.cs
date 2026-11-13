using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CuentaSaldoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaSaldoInicial",
                table: "cuentas",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoInicial",
                table: "cuentas",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "cuentas",
                keyColumn: "Id_Cuenta",
                keyValue: 1,
                columns: new[] { "FechaSaldoInicial", "SaldoInicial" },
                values: new object[] { null, 0m });

            migrationBuilder.UpdateData(
                table: "cuentas",
                keyColumn: "Id_Cuenta",
                keyValue: 2,
                columns: new[] { "FechaSaldoInicial", "SaldoInicial" },
                values: new object[] { null, 0m });

            migrationBuilder.UpdateData(
                table: "cuentas",
                keyColumn: "Id_Cuenta",
                keyValue: 3,
                columns: new[] { "FechaSaldoInicial", "SaldoInicial" },
                values: new object[] { null, 0m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaSaldoInicial",
                table: "cuentas");

            migrationBuilder.DropColumn(
                name: "SaldoInicial",
                table: "cuentas");
        }
    }
}
