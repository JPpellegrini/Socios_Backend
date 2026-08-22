using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Socios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedUpper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "caja",
                keyColumn: "Id_Movimiento",
                keyValue: 1,
                columns: new[] { "Observacion", "Tipo_Movimiento" },
                values: new object[] { "PAGO CUOTA MAYO", "INGRESO" });

            migrationBuilder.UpdateData(
                table: "caja",
                keyColumn: "Id_Movimiento",
                keyValue: 2,
                columns: new[] { "Observacion", "Tipo_Movimiento" },
                values: new object[] { "PAGO GAS MES JUNIO", "EGRESO" });

            migrationBuilder.UpdateData(
                table: "caja",
                keyColumn: "Id_Movimiento",
                keyValue: 3,
                columns: new[] { "Observacion", "Tipo_Movimiento" },
                values: new object[] { "COLABORACIÓN PODOLOGÍA", "INGRESO" });

            migrationBuilder.UpdateData(
                table: "cierres_contables",
                keyColumn: "Id_Cierre",
                keyValue: 1,
                column: "Tipo",
                value: "MENSUAL");

            migrationBuilder.UpdateData(
                table: "cierres_contables",
                keyColumn: "Id_Cierre",
                keyValue: 2,
                column: "Tipo",
                value: "ANUAL");

            migrationBuilder.UpdateData(
                table: "ciudades",
                keyColumn: "Id_Ciudad",
                keyValue: 1,
                column: "Nombre",
                value: "ROLDÁN");

            migrationBuilder.UpdateData(
                table: "ciudades",
                keyColumn: "Id_Ciudad",
                keyValue: 2,
                column: "Nombre",
                value: "FUNES");

            migrationBuilder.UpdateData(
                table: "ciudades",
                keyColumn: "Id_Ciudad",
                keyValue: 3,
                column: "Nombre",
                value: "ROSARIO");

            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 1,
                column: "Tipo",
                value: "TELÉFONO");

            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 2,
                column: "Tipo",
                value: "MAIL");

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

            migrationBuilder.UpdateData(
                table: "cuentas",
                keyColumn: "Id_Cuenta",
                keyValue: 1,
                columns: new[] { "Estado", "NombreCuenta" },
                values: new object[] { "ACTIVA", "EFECTIVO" });

            migrationBuilder.UpdateData(
                table: "cuentas",
                keyColumn: "Id_Cuenta",
                keyValue: 2,
                columns: new[] { "Estado", "NombreCuenta" },
                values: new object[] { "ACTIVA", "BANCO MACRO" });

            migrationBuilder.UpdateData(
                table: "cuentas",
                keyColumn: "Id_Cuenta",
                keyValue: 3,
                columns: new[] { "Estado", "NombreCuenta" },
                values: new object[] { "INACTIVA", "BANCO NACION" });

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 1,
                column: "NombreDetalleMovimiento",
                value: "CUOTA SOCIO");

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 2,
                column: "NombreDetalleMovimiento",
                value: "CUOTA SEPELIO");

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 3,
                column: "NombreDetalleMovimiento",
                value: "AJUSTE DIFERENCIA CAJA");

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 4,
                column: "NombreDetalleMovimiento",
                value: "COLABORACIÓN");

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 5,
                column: "NombreDetalleMovimiento",
                value: "PAGO A PROVEEDOR");

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 6,
                column: "NombreDetalleMovimiento",
                value: "TRANSFERENCIA");

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 1,
                columns: new[] { "Apellido", "Calle", "Nombre", "Sexo" },
                values: new object[] { "OLDAN", "INDEPENDENCIA", "LUCIANO", "HOMBRE" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 2,
                columns: new[] { "Apellido", "Calle", "Nombre", "Sexo" },
                values: new object[] { "PÉREZ", "GALINDO", "JUAN", "HOMBRE" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 3,
                columns: new[] { "Calle", "RazonSocial", "Sexo" },
                values: new object[] { "MITRE", "LITORAL GAS S.A", "PERSONA JURIDICA" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 4,
                columns: new[] { "Apellido", "Calle", "Nombre", "Sexo" },
                values: new object[] { "LÓPEZ", "LAS HERAS", "MARÍA", "MUJER" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 5,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "SALTA", "HOMBRE" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 6,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "INDEPENDENCIA", "HOMBRE" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 7,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "MENDOZA", "HOMBRE" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 8,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "CORRIENTES", "HOMBRE" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 9,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "MENDOZA", "HOMBRE" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 10,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "SALTA", "MUJER" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 11,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "1 DE MAYO", "MUJER" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 12,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "MITRE", "MUJER" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 13,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "JUJUY", "MUJER" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 14,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "JUJUY", "MUJER" });

            migrationBuilder.UpdateData(
                table: "estado_cajadiaria",
                keyColumn: "Id_CajaDiaria",
                keyValue: 1,
                column: "Tipo",
                value: "APERTURA");

            migrationBuilder.UpdateData(
                table: "estado_cajadiaria",
                keyColumn: "Id_CajaDiaria",
                keyValue: 2,
                column: "Tipo",
                value: "CIERRE");

            migrationBuilder.UpdateData(
                table: "estado_cajadiaria",
                keyColumn: "Id_CajaDiaria",
                keyValue: 3,
                column: "Tipo",
                value: "APERTURA");

            migrationBuilder.UpdateData(
                table: "estado_cajadiaria",
                keyColumn: "Id_CajaDiaria",
                keyValue: 4,
                column: "Tipo",
                value: "CIERRE");

            migrationBuilder.UpdateData(
                table: "metodos_pago",
                keyColumn: "Id_MetodoPago",
                keyValue: 1,
                column: "NombreMetodoPago",
                value: "EFECTIVO");

            migrationBuilder.UpdateData(
                table: "metodos_pago",
                keyColumn: "Id_MetodoPago",
                keyValue: 2,
                column: "NombreMetodoPago",
                value: "TRANSFERENCIA BANCARIA");

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 1,
                column: "NombrePrestacion",
                value: "PODOLOGÍA");

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 2,
                column: "NombrePrestacion",
                value: "CARDIOLOGÍA");

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 3,
                column: "NombrePrestacion",
                value: "ALQUILER SALÓN");

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 4,
                column: "NombrePrestacion",
                value: "COMISIÓN COBRADOR");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id_Rol",
                keyValue: 1,
                columns: new[] { "Descripcion", "RolNombre" },
                values: new object[] { "SECRETARIA QUE MANEJA TODO EL SISTEMA", "SECRETARIA" });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id_Rol",
                keyValue: 2,
                columns: new[] { "Descripcion", "RolNombre" },
                values: new object[] { "SOLO POSEE ACCESO AL MÓDULO DE INFORMES", "CONSULTOR" });

            migrationBuilder.UpdateData(
                table: "tipo_entidades",
                keyColumn: "Id_Tipo",
                keyValue: 1,
                column: "NombreTipoEntidad",
                value: "SOCIO");

            migrationBuilder.UpdateData(
                table: "tipo_entidades",
                keyColumn: "Id_Tipo",
                keyValue: 2,
                column: "NombreTipoEntidad",
                value: "PROVEEDOR");

            migrationBuilder.UpdateData(
                table: "tipo_entidades",
                keyColumn: "Id_Tipo",
                keyValue: 3,
                column: "NombreTipoEntidad",
                value: "COLABORADOR");

            migrationBuilder.UpdateData(
                table: "tipo_entidades",
                keyColumn: "Id_Tipo",
                keyValue: 4,
                column: "NombreTipoEntidad",
                value: "EMPLEADO");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 2,
                column: "UsuarioNombre",
                value: "ADRIANA");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 3,
                column: "UsuarioNombre",
                value: "MARCELA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "caja",
                keyColumn: "Id_Movimiento",
                keyValue: 1,
                columns: new[] { "Observacion", "Tipo_Movimiento" },
                values: new object[] { "Pago Cuota Mayo", "Ingreso" });

            migrationBuilder.UpdateData(
                table: "caja",
                keyColumn: "Id_Movimiento",
                keyValue: 2,
                columns: new[] { "Observacion", "Tipo_Movimiento" },
                values: new object[] { "Pago Gas Mes Junio", "Egreso" });

            migrationBuilder.UpdateData(
                table: "caja",
                keyColumn: "Id_Movimiento",
                keyValue: 3,
                columns: new[] { "Observacion", "Tipo_Movimiento" },
                values: new object[] { "Colaboración Podología", "Ingreso" });

            migrationBuilder.UpdateData(
                table: "cierres_contables",
                keyColumn: "Id_Cierre",
                keyValue: 1,
                column: "Tipo",
                value: "Mensual");

            migrationBuilder.UpdateData(
                table: "cierres_contables",
                keyColumn: "Id_Cierre",
                keyValue: 2,
                column: "Tipo",
                value: "Anual");

            migrationBuilder.UpdateData(
                table: "ciudades",
                keyColumn: "Id_Ciudad",
                keyValue: 1,
                column: "Nombre",
                value: "Roldán");

            migrationBuilder.UpdateData(
                table: "ciudades",
                keyColumn: "Id_Ciudad",
                keyValue: 2,
                column: "Nombre",
                value: "Funes");

            migrationBuilder.UpdateData(
                table: "ciudades",
                keyColumn: "Id_Ciudad",
                keyValue: 3,
                column: "Nombre",
                value: "Rosario");

            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 1,
                column: "Tipo",
                value: "Celular");

            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 2,
                column: "Tipo",
                value: "Mail");

            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 3,
                column: "Tipo",
                value: "Celular");

            migrationBuilder.UpdateData(
                table: "contactos",
                keyColumn: "Id_Contacto",
                keyValue: 4,
                column: "Tipo",
                value: "Emergencia");

            migrationBuilder.UpdateData(
                table: "cuentas",
                keyColumn: "Id_Cuenta",
                keyValue: 1,
                columns: new[] { "Estado", "NombreCuenta" },
                values: new object[] { "Activa", "Efectivo" });

            migrationBuilder.UpdateData(
                table: "cuentas",
                keyColumn: "Id_Cuenta",
                keyValue: 2,
                columns: new[] { "Estado", "NombreCuenta" },
                values: new object[] { "Activa", "Banco Macro" });

            migrationBuilder.UpdateData(
                table: "cuentas",
                keyColumn: "Id_Cuenta",
                keyValue: 3,
                columns: new[] { "Estado", "NombreCuenta" },
                values: new object[] { "Inactiva", "Banco Macro 2" });

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 1,
                column: "NombreDetalleMovimiento",
                value: "Cuota Socio");

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 2,
                column: "NombreDetalleMovimiento",
                value: "Cuota Sepelio");

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 3,
                column: "NombreDetalleMovimiento",
                value: "Ajuste Diferencia Caja");

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 4,
                column: "NombreDetalleMovimiento",
                value: "Colaboración");

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 5,
                column: "NombreDetalleMovimiento",
                value: "Pago a Proveedor");

            migrationBuilder.UpdateData(
                table: "detalle_movimientos",
                keyColumn: "Id_Detmov",
                keyValue: 6,
                column: "NombreDetalleMovimiento",
                value: "Transferencia");

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 1,
                columns: new[] { "Apellido", "Calle", "Nombre", "Sexo" },
                values: new object[] { "Oldan", "Independencia", "Luciano", "Hombre" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 2,
                columns: new[] { "Apellido", "Calle", "Nombre", "Sexo" },
                values: new object[] { "Pérez", "Galindo", "Juan", "Hombre" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 3,
                columns: new[] { "Calle", "RazonSocial", "Sexo" },
                values: new object[] { "Mitre", "Litoral Gas S.A", "Persona Juridica" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 4,
                columns: new[] { "Apellido", "Calle", "Nombre", "Sexo" },
                values: new object[] { "López", "Las Heras", "María", "Mujer" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 5,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "Salta", "Hombre" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 6,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "Independencia", "Hombre" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 7,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "Mendoza", "Hombre" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 8,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "Corrientes", "Hombre" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 9,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "Mendoza", "Hombre" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 10,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "Salta", "Mujer" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 11,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "1 de Mayo", "Mujer" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 12,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "Mitre", "Mujer" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 13,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "Jujuy", "Mujer" });

            migrationBuilder.UpdateData(
                table: "entidades",
                keyColumn: "Id_Entidad",
                keyValue: 14,
                columns: new[] { "Calle", "Sexo" },
                values: new object[] { "Jujuy", "Mujer" });

            migrationBuilder.UpdateData(
                table: "estado_cajadiaria",
                keyColumn: "Id_CajaDiaria",
                keyValue: 1,
                column: "Tipo",
                value: "Apertura");

            migrationBuilder.UpdateData(
                table: "estado_cajadiaria",
                keyColumn: "Id_CajaDiaria",
                keyValue: 2,
                column: "Tipo",
                value: "Cierre");

            migrationBuilder.UpdateData(
                table: "estado_cajadiaria",
                keyColumn: "Id_CajaDiaria",
                keyValue: 3,
                column: "Tipo",
                value: "Apertura");

            migrationBuilder.UpdateData(
                table: "estado_cajadiaria",
                keyColumn: "Id_CajaDiaria",
                keyValue: 4,
                column: "Tipo",
                value: "Cierre");

            migrationBuilder.UpdateData(
                table: "metodos_pago",
                keyColumn: "Id_MetodoPago",
                keyValue: 1,
                column: "NombreMetodoPago",
                value: "Efectivo");

            migrationBuilder.UpdateData(
                table: "metodos_pago",
                keyColumn: "Id_MetodoPago",
                keyValue: 2,
                column: "NombreMetodoPago",
                value: "Transferencia Bancaria");

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 1,
                column: "NombrePrestacion",
                value: "Podología");

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 2,
                column: "NombrePrestacion",
                value: "Cardiología");

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 3,
                column: "NombrePrestacion",
                value: "Alquiler Salón");

            migrationBuilder.UpdateData(
                table: "prestaciones",
                keyColumn: "Id_Prestacion",
                keyValue: 4,
                column: "NombrePrestacion",
                value: "Comisión Cobrador");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id_Rol",
                keyValue: 1,
                columns: new[] { "Descripcion", "RolNombre" },
                values: new object[] { "Secretaria que maneja todo el sistema", "Secretaria" });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "Id_Rol",
                keyValue: 2,
                columns: new[] { "Descripcion", "RolNombre" },
                values: new object[] { "Solo posee acceso al módulo de informes", "Consultor" });

            migrationBuilder.UpdateData(
                table: "tipo_entidades",
                keyColumn: "Id_Tipo",
                keyValue: 1,
                column: "NombreTipoEntidad",
                value: "Socio");

            migrationBuilder.UpdateData(
                table: "tipo_entidades",
                keyColumn: "Id_Tipo",
                keyValue: 2,
                column: "NombreTipoEntidad",
                value: "Proveedor");

            migrationBuilder.UpdateData(
                table: "tipo_entidades",
                keyColumn: "Id_Tipo",
                keyValue: 3,
                column: "NombreTipoEntidad",
                value: "Colaborador");

            migrationBuilder.UpdateData(
                table: "tipo_entidades",
                keyColumn: "Id_Tipo",
                keyValue: 4,
                column: "NombreTipoEntidad",
                value: "Empleado");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 2,
                column: "UsuarioNombre",
                value: "Adriana");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id_Usuario",
                keyValue: 3,
                column: "UsuarioNombre",
                value: "Marcela");
        }
    }
}
