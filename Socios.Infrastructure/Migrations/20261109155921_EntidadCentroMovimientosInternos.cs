using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <summary>
    /// Crea la entidad del propio Centro, a la que se imputan los movimientos internos de caja
    /// (transferencias entre cuentas y, más adelante, ajustes de diferencia): la columna
    /// caja.Id_Entidad es obligatoria y esos movimientos no tienen un socio ni un proveedor.
    ///
    /// Se inserta con SQL y no con HasData para que el Id lo asigne la secuencia de identidad:
    /// un Id fijo no avanza la secuencia y un alta futura podría chocar con él. El código la
    /// ubica por su razón social exacta (ver CajaDiariaUseCase.RazonSocialCentro).
    /// No se carga CUIT: no está informado.
    /// </summary>
    public partial class EntidadCentroMovimientosInternos : Migration
    {
        private const string RazonSocial = "CENTRO DE JUBILADOS Y PENSIONADOS DE ROLDÁN";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
                INSERT INTO entidades ("Tipo", "RazonSocial", "Id_Ciudad", "Calle", "Altura", "Observacion")
                SELECT 'CUIT', '{RazonSocial}', 1, 'INDEPENDENCIA', 176,
                       'ENTIDAD DEL CENTRO PARA MOVIMIENTOS INTERNOS DE CAJA (TRANSFERENCIAS Y AJUSTES)'
                WHERE NOT EXISTS (SELECT 1 FROM entidades WHERE "RazonSocial" = '{RazonSocial}');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Solo si ningún movimiento la usa: con movimientos registrados no se puede quitar.
            migrationBuilder.Sql($"""
                DELETE FROM entidades e
                WHERE e."RazonSocial" = '{RazonSocial}'
                  AND NOT EXISTS (SELECT 1 FROM caja c WHERE c."Id_Entidad" = e."Id_Entidad");
                """);
        }
    }
}
