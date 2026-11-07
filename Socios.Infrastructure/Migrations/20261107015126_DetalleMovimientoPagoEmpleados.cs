using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DetalleMovimientoPagoEmpleados : Migration
    {
        /// <summary>
        /// Agrega el detalle de movimiento 8 "PAGO A EMPLEADOS" (mismo valor que el HasData de
        /// SociosDbContext). Se escribe con SQL en lugar de InsertData para tolerar bases de
        /// desarrollo donde el concepto ya se creó a mano con un script de prueba:
        ///   - Si el Id 8 no existe, lo inserta.
        ///   - Si existe con el mismo nombre, no hace nada.
        ///   - Si existe con OTRO nombre, falla: no se puede pisar un concepto que puede tener
        ///     movimientos de caja asociados; hay que revisarlo a mano.
        /// Al final adelanta la secuencia de Id_Detmov para que el próximo alta no choque.
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                DECLARE
                    v_nombre TEXT;
                BEGIN
                    SELECT "NombreDetalleMovimiento" INTO v_nombre
                    FROM detalle_movimientos WHERE "Id_Detmov" = 8;

                    IF v_nombre IS NULL THEN
                        INSERT INTO detalle_movimientos ("Id_Detmov", "NombreDetalleMovimiento")
                        VALUES (8, 'PAGO A EMPLEADOS');
                    ELSIF v_nombre <> 'PAGO A EMPLEADOS' THEN
                        RAISE EXCEPTION 'El detalle de movimiento 8 ya existe como "%". Revíselo antes de aplicar la migración DetalleMovimientoPagoEmpleados.', v_nombre;
                    END IF;
                END $$;

                SELECT setval(
                    pg_get_serial_sequence('detalle_movimientos', 'Id_Detmov'),
                    (SELECT MAX("Id_Detmov") FROM detalle_movimientos));
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 8);
        }
    }
}
