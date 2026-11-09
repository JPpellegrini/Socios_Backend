-- Datos de PRUEBA para el reporte de movimientos por entidad, proveedor 1001
-- (Distribuidora del Litoral S.A., cargado por la migración SeedProveedores).
-- Solo para la base local de desarrollo. No ejecutar en producción.
--
-- Valores de catálogo usados (verificados en la base local):
--   métodos de pago:  1 EFECTIVO, 2 TRANSFERENCIA BANCARIA
--   cuentas:          1 EFECTIVO, 2 BANCO MACRO (789012)
--   detalle mov.:     5 PAGO A PROVEEDOR (único concepto que lista el reporte para proveedores)
--                     3 AJUSTE DIFERENCIA CAJA (se carga uno a propósito: NO debe aparecer)
--   caja diaria:      1 (caja 1), 3 (caja 2)
-- Id_Movimiento se omite: lo asigna la base.

BEGIN;

-- Los movimientos de ejemplo se cargaron con Id explícito (seed), así que la secuencia puede
-- haber quedado atrasada. Se la adelanta al máximo actual para evitar errores de clave duplicada.
SELECT setval(pg_get_serial_sequence('caja', 'Id_Movimiento'), (SELECT MAX("Id_Movimiento") FROM caja));

INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto", "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion") VALUES
('2026-04-01 10:00', 1, 'EGRESO',  1001, 180000.00, 2, 2, 5, 'FACTURA ABRIL 2026'),
('2026-05-04 11:15', 1, 'EGRESO',  1001, 185000.00, 2, 2, 5, 'FACTURA MAYO 2026'),
('2026-05-20 09:30', 3, 'INGRESO', 1001,   5000.00, 1, 1, 5, 'REINTEGRO POR DIFERENCIA EN FACTURA MAYO'),
('2026-06-02 12:00', 1, 'EGRESO',  1001, 190000.00, 1, 1, 5, 'FACTURA JUNIO 2026'),
('2026-07-03 10:45', 3, 'EGRESO',  1001, 190000.00, 2, 2, 5, 'FACTURA JULIO 2026'),
('2026-08-04 09:10', 1, 'EGRESO',  1001, 195000.00, 2, 2, 5, 'FACTURA AGOSTO 2026'),
('2026-09-02 11:20', 3, 'EGRESO',  1001, 195000.00, 2, 2, 5, NULL),
('2026-10-01 10:05', 1, 'EGRESO',  1001, 200000.00, 1, 1, 5, 'FACTURA OCTUBRE 2026'),
('2026-10-15 16:40', 3, 'EGRESO',  1001,  99999.00, 1, 1, 3, 'NO DEBE APARECER EN EL REPORTE: OTRO CONCEPTO');

COMMIT;

-- Control: 8 movimientos de PAGO A PROVEEDOR.
-- Con el período 01/01/2026 al 31/12/2026, el reporte debe mostrar TOTAL EGRESO $ 1.330.000,00
-- (egresos 1.335.000,00 menos el reintegro de 5.000,00).
SELECT COUNT(*) AS movimientos,
       SUM(CASE WHEN "Tipo_Movimiento" = 'EGRESO' THEN "Monto" ELSE -"Monto" END) AS total_egreso_neto
FROM caja
WHERE "Id_Entidad" = 1001
  AND "Id_Detmov" = 5;

-- Para borrar estos datos de prueba (el proveedor 1001 no tenía movimientos antes del script):
-- DELETE FROM caja WHERE "Id_Entidad" = 1001;
