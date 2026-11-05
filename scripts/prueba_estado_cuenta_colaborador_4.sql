-- Datos de PRUEBA para el reporte de movimientos por entidad, colaborador 4 (LÓPEZ, MARÍA).
-- Solo para la base local de desarrollo. No ejecutar en producción.
--
-- Valores de catálogo usados (verificados en la base local):
--   métodos de pago:  1 EFECTIVO, 2 TRANSFERENCIA BANCARIA
--   cuentas:          1 EFECTIVO, 2 BANCO MACRO (789012)
--   detalle mov.:     4 COLABORACIÓN (único concepto que lista el reporte para colaboradores)
--   caja diaria:      1 (caja 1), 3 (caja 2)
-- Id_Movimiento se omite: lo asigna la base.
-- Se incluyen dos EGRESOS (devoluciones) para ver en el PDF los importes negativos que restan.

BEGIN;

-- Los movimientos de ejemplo se cargaron con Id explícito (seed), así que la secuencia puede
-- haber quedado atrasada. Se la adelanta al máximo actual para evitar errores de clave duplicada.
SELECT setval(pg_get_serial_sequence('caja', 'Id_Movimiento'), (SELECT MAX("Id_Movimiento") FROM caja));

INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto", "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion") VALUES
('2026-07-08 10:15', 1, 'INGRESO', 4, 30000.00, 1, 1, 4, 'COLABORACIÓN PODOLOGÍA JULIO'),
('2026-07-22 11:40', 3, 'INGRESO', 4, 15000.00, 2, 2, 4, 'COLABORACIÓN CARDIOLOGÍA'),
('2026-08-05 09:05', 1, 'INGRESO', 4, 30000.00, 2, 2, 4, 'COLABORACIÓN PODOLOGÍA AGOSTO'),
('2026-08-19 16:30', 3, 'EGRESO',  4,  5000.00, 1, 1, 4, 'DEVOLUCIÓN POR DIFERENCIA'),
('2026-09-03 10:00', 1, 'INGRESO', 4, 32500.00, 1, 1, 4, 'COLABORACIÓN PODOLOGÍA SEPTIEMBRE'),
('2026-09-17 12:20', 3, 'INGRESO', 4, 18000.00, 2, 2, 4, 'COLABORACIÓN CARDIOLOGÍA'),
('2026-10-02 09:45', 1, 'INGRESO', 4, 32500.00, 2, 2, 4, 'COLABORACIÓN PODOLOGÍA OCTUBRE'),
('2026-10-16 15:10', 3, 'EGRESO',  4,  2500.00, 1, 1, 4, 'DEVOLUCIÓN'),
('2026-10-30 11:00', 1, 'INGRESO', 4, 20000.00, 1, 1, 4, NULL),
('2026-11-03 10:30', 3, 'INGRESO', 4, 35000.00, 2, 2, 4, 'COLABORACIÓN PODOLOGÍA NOVIEMBRE');

COMMIT;

-- Control: 11 movimientos de COLABORACIÓN (1 existente + 10 nuevos).
-- Con el período 01/01/2026 al 31/12/2026, el reporte debe mostrar TOTAL INGRESO $ 235.500,00
-- (ingresos 243.000,00 menos devoluciones 7.500,00).
SELECT COUNT(*) AS movimientos,
       SUM(CASE WHEN "Tipo_Movimiento" = 'INGRESO' THEN "Monto" ELSE -"Monto" END) AS total_ingreso_neto
FROM caja
WHERE "Id_Entidad" = 4
  AND "Id_Detmov" = 4;

-- Para borrar estos datos de prueba:
-- DELETE FROM caja
-- WHERE "Id_Entidad" = 4
--   AND "Id_Movimiento" <> 3
--   AND "FechaHoraMov" BETWEEN '2026-07-08' AND '2026-11-03 23:59';
