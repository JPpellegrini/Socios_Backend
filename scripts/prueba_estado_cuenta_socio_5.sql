-- Datos de PRUEBA para el estado de cuenta del socio AVILA, EDUARDO (socio 4, entidad 5).
-- Solo para la base local de desarrollo. No ejecutar en producción.
-- Antes del script, la entidad 5 no tenía cuotas ni movimientos de caja.
--
-- Valores de catálogo usados (verificados en la base local):
--   tipos de cuota:   1 SOCIO ($ 4.500), 4 SEPELIO plan A "más de 65" ($ 12.000; nació en 1958)
--   detalle mov.:     1 CUOTA SOCIO, 2 CUOTA SEPELIO
--   métodos de pago:  1 EFECTIVO (cuenta 1 EFECTIVO), 2 TRANSFERENCIA BANCARIA (cuenta 2 BANCO MACRO)
--   caja diaria:      1
--
-- Casos que cubre:
--   - Enero a mayo 2026: SOCIO y SEPELIO pagadas (con su movimiento de caja vinculado).
--   - Junio: SOCIO pagada; SEPELIO VENCIDA con monto viejo $ 10.000 (el reporte la informa a $ 12.000 *).
--   - Julio: SOCIO VENCIDA con monto viejo $ 4.000 (se informa a $ 4.500 *); SEPELIO PENDIENTE.
--   - Agosto: SOCIO ANULADA (no aparece) y regenerada y pagada; SEPELIO PENDIENTE.
--   - Septiembre y octubre: SOCIO y SEPELIO PENDIENTES.
--   - Octubre 2025: SOCIO VENCIDA con vencimiento 10/11/2025 (fuera del período 2026: no aparece
--     en la tabla, pero sí suma en la "deuda total del socio a la fecha").

BEGIN;

-- Las secuencias pueden haber quedado atrasadas por los datos semilla con Id explícito.
SELECT setval(pg_get_serial_sequence('caja', 'Id_Movimiento'), (SELECT MAX("Id_Movimiento") FROM caja));
SELECT setval(pg_get_serial_sequence('cuotas', 'Id_Deuda'), COALESCE((SELECT MAX("Id_Deuda") FROM cuotas), 1));

-- 1) Cuotas PAGADAS: por cada una se registra el ingreso en caja y la cuota queda vinculada.
DO $$
DECLARE
    r RECORD;
    v_mov INTEGER;
    v_meses TEXT[] := ARRAY['ENE','FEB','MAR','ABR','MAY','JUN','JUL','AGO','SEP','OCT','NOV','DIC'];
BEGIN
    FOR r IN
        SELECT * FROM (VALUES
            -- mes, tipo cuota, concepto, detalle mov., monto, fecha de pago, método, cuenta
            (1, 1, 'SOCIO',   1,  4500.00, TIMESTAMP '2026-02-05 10:15', 1, 1),
            (1, 4, 'SEPELIO', 2, 12000.00, TIMESTAMP '2026-02-05 10:16', 1, 1),
            (2, 1, 'SOCIO',   1,  4500.00, TIMESTAMP '2026-03-04 11:30', 2, 2),
            (2, 4, 'SEPELIO', 2, 12000.00, TIMESTAMP '2026-03-04 11:31', 2, 2),
            (3, 1, 'SOCIO',   1,  4500.00, TIMESTAMP '2026-04-01 09:45', 2, 2),
            (3, 4, 'SEPELIO', 2, 12000.00, TIMESTAMP '2026-04-01 09:46', 2, 2),
            (4, 1, 'SOCIO',   1,  4500.00, TIMESTAMP '2026-05-08 12:10', 1, 1),
            (4, 4, 'SEPELIO', 2, 12000.00, TIMESTAMP '2026-05-08 12:11', 1, 1),
            (5, 1, 'SOCIO',   1,  4500.00, TIMESTAMP '2026-06-09 10:00', 2, 2),
            (5, 4, 'SEPELIO', 2, 12000.00, TIMESTAMP '2026-06-09 10:01', 2, 2),
            (6, 1, 'SOCIO',   1,  4500.00, TIMESTAMP '2026-07-06 16:20', 1, 1),
            (8, 1, 'SOCIO',   1,  4500.00, TIMESTAMP '2026-09-03 10:40', 2, 2)
        ) AS t(mes, id_tipocuota, concepto, id_detmov, monto, fecha_pago, id_metodo, id_cuenta)
    LOOP
        INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                          "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
        VALUES (r.fecha_pago, 1, 'INGRESO', 5, r.monto, r.id_metodo, r.id_cuenta, r.id_detmov,
                'PAGO CUOTA ' || r.concepto || ' ' || v_meses[r.mes] || ' 2026')
        RETURNING "Id_Movimiento" INTO v_mov;

        INSERT INTO cuotas ("Id_Entidad", "Mes_Periodo", "Anio_Periodo", "Id_TipoCuota", "Monto",
                            "FechaHoraGeneracion", "FechaVencimiento", "Estado", "Id_Movimiento")
        VALUES (5, r.mes, 2026, r.id_tipocuota, r.monto,
                make_date(2026, r.mes, 1),
                make_date(2026, r.mes, 1) + INTERVAL '1 month' + INTERVAL '9 days',  -- día 10 del mes siguiente
                'PAGADO', v_mov);
    END LOOP;
END $$;

-- 2) Cuotas ADEUDADAS y ANULADA (sin movimiento de caja).
INSERT INTO cuotas ("Id_Entidad", "Mes_Periodo", "Anio_Periodo", "Id_TipoCuota", "Monto",
                    "FechaHoraGeneracion", "FechaVencimiento", "Estado", "Id_Movimiento") VALUES
(5,  6, 2026, 4, 10000.00, '2026-06-01', '2026-07-10', 'VENCIDA',   NULL),
(5,  7, 2026, 1,  4000.00, '2026-07-01', '2026-08-10', 'VENCIDA',   NULL),
(5,  7, 2026, 4, 12000.00, '2026-07-01', '2026-08-10', 'PENDIENTE', NULL),
(5,  8, 2026, 1,  4500.00, '2026-08-01', '2026-09-10', 'ANULADA',   NULL),
(5,  8, 2026, 4, 12000.00, '2026-08-01', '2026-09-10', 'PENDIENTE', NULL),
(5,  9, 2026, 1,  4500.00, '2026-09-01', '2026-10-10', 'PENDIENTE', NULL),
(5,  9, 2026, 4, 12000.00, '2026-09-01', '2026-10-10', 'PENDIENTE', NULL),
(5, 10, 2026, 1,  4500.00, '2026-10-01', '2026-11-10', 'PENDIENTE', NULL),
(5, 10, 2026, 4, 12000.00, '2026-10-01', '2026-11-10', 'PENDIENTE', NULL),
(5, 10, 2025, 1,  3800.00, '2025-10-01', '2025-11-10', 'VENCIDA',   NULL);

COMMIT;

-- Control. Con el período 01/01/2026 al 31/12/2026 el reporte debe mostrar:
--   20 cuotas | TOTAL $ 165.000,00 | Pagado $ 91.500,00 | Adeudado $ 73.500,00
--   Deuda total del socio a la fecha: $ 78.000,00 (9 cuotas adeudadas)
SELECT "Estado", COUNT(*) AS cuotas, SUM("Monto") AS monto_guardado
FROM cuotas
WHERE "Id_Entidad" = 5
GROUP BY "Estado"
ORDER BY "Estado";

-- Para borrar estos datos de prueba (primero las cuotas, que referencian a caja):
-- DELETE FROM cuotas WHERE "Id_Entidad" = 5;
-- DELETE FROM caja   WHERE "Id_Entidad" = 5;
