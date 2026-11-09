-- Datos de PRUEBA: pagos a empleados y cuotas de nicho (2026).
-- Solo para la base local de desarrollo. No ejecutar en producción.
--
-- 1) PAGO A EMPLEADOS
--    El concepto "PAGO A EMPLEADOS" (Id 8) lo crea la migración DetalleMovimientoPagoEmpleados:
--    aplique las migraciones antes de ejecutar este script (si falta, el script se detiene).
--    Sueldos de enero a octubre (pagados el día 5 del mes siguiente, por transferencia):
--      15 LÓPEZ, ADRIANA      $ 180.000 por mes
--      16 RODRIGUEZ, MARCELA  $ 150.000 por mes
--      17 DE PAUL, RODRIGO    $ 120.000 por mes, solo enero a marzo (hoy está INACTIVO)
--
-- 2) CUOTAS DE NICHO
--    Se asigna el nicho B-1 (Id 2, libre) a IPPOLITI, HUGO (entidad 9: socio activo con sepelio
--    y sin otro nicho, como exige el sistema), simulando una asignación del 02/03/2026:
--      valor $ 2.500.000, sin lápida, 12 cuotas con 2,5 % de interés.
--      Monto de cada cuota = 2.500.000 / 12 × 1,025 = $ 213.541,67 (misma fórmula que NichoUseCase).
--      Períodos marzo 2026 a febrero 2027; la primera vence el 10/04/2026 y las siguientes,
--      un mes más cada una. Todas comparten FechaHoraGeneracion (identifica el lote).
--    Pagadas: marzo a agosto (6 cuotas, ingreso en caja con el concepto CUOTA NICHO).
--    Septiembre queda VENCIDA (venció el 10/10); el resto, PENDIENTE.
--
-- Si el nicho 2 ya está asignado o el concepto ya tiene pagos, el script se detiene sin cargar nada.

BEGIN;

-- Las secuencias pueden haber quedado atrasadas por los datos semilla con Id explícito.
SELECT setval(pg_get_serial_sequence('caja', 'Id_Movimiento'), (SELECT MAX("Id_Movimiento") FROM caja));
SELECT setval(pg_get_serial_sequence('cuotas', 'Id_Deuda'), COALESCE((SELECT MAX("Id_Deuda") FROM cuotas), 1));

DO $$
DECLARE
    v_meses TEXT[] := ARRAY['ENE','FEB','MAR','ABR','MAY','JUN','JUL','AGO','SEP','OCT','NOV','DIC'];
    v_det_empleados INTEGER;
    v_det_nicho INTEGER;
    v_tc_nicho INTEGER;
    v_m INTEGER;
    v_i INTEGER;
    v_mov INTEGER;
    v_periodo DATE;
    v_vencimiento DATE;
    v_estado TEXT;
    v_monto_cuota NUMERIC := ROUND(2500000.0 / 12 * 1.025, 2);  -- 213.541,67
    v_generacion TIMESTAMP := TIMESTAMP '2026-03-02 10:00:00';
BEGIN
    SELECT "Id_Detmov" INTO v_det_empleados FROM detalle_movimientos WHERE "NombreDetalleMovimiento" = 'PAGO A EMPLEADOS';
    SELECT "Id_Detmov" INTO v_det_nicho FROM detalle_movimientos WHERE "NombreDetalleMovimiento" = 'CUOTA NICHO';
    SELECT "Id_TipoCuota" INTO v_tc_nicho FROM tipo_cuotas WHERE "Concepto" = 'NICHO';

    IF v_det_empleados IS NULL OR v_det_nicho IS NULL OR v_tc_nicho IS NULL THEN
        RAISE EXCEPTION 'Faltan los conceptos PAGO A EMPLEADOS / CUOTA NICHO o el tipo de cuota NICHO: aplique las migraciones.';
    END IF;

    -- Protección: no duplicar si el script ya se ejecutó.
    IF EXISTS (SELECT 1 FROM caja WHERE "Id_Detmov" = v_det_empleados) THEN
        RAISE EXCEPTION 'Ya hay pagos a empleados cargados: el script parece ya ejecutado.';
    END IF;
    IF EXISTS (SELECT 1 FROM nichos WHERE "Id_Nicho" = 2 AND ("Id_Entidad" IS NOT NULL OR "Ocupado" = 'SI')) THEN
        RAISE EXCEPTION 'El nicho 2 ya está asignado u ocupado: el script parece ya ejecutado.';
    END IF;
    IF EXISTS (SELECT 1 FROM nichos WHERE "Id_Entidad" = 9) THEN
        RAISE EXCEPTION 'La entidad 9 ya tiene un nicho asignado.';
    END IF;

    ---------------------------------------------------------------------------------------------
    -- 1) Sueldos (EGRESO).
    ---------------------------------------------------------------------------------------------
    FOR v_m IN 1..10 LOOP
        INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                          "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion") VALUES
        (make_date(2026, v_m, 1) + INTERVAL '1 month' + INTERVAL '4 days' + TIME '12:00', 1, 'EGRESO', 15, 180000, 2, 2,
         v_det_empleados, 'SUELDO ' || v_meses[v_m] || ' 2026'),
        (make_date(2026, v_m, 1) + INTERVAL '1 month' + INTERVAL '4 days' + TIME '12:05', 1, 'EGRESO', 16, 150000, 2, 2,
         v_det_empleados, 'SUELDO ' || v_meses[v_m] || ' 2026');

        IF v_m <= 3 THEN
            INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                              "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
            VALUES (make_date(2026, v_m, 1) + INTERVAL '1 month' + INTERVAL '4 days' + TIME '12:10', 1, 'EGRESO', 17,
                    120000, 2, 2, v_det_empleados, 'SUELDO ' || v_meses[v_m] || ' 2026');
        END IF;
    END LOOP;

    ---------------------------------------------------------------------------------------------
    -- 2) Asignación del nicho B-1 a la entidad 9 y sus 12 cuotas.
    ---------------------------------------------------------------------------------------------
    UPDATE nichos
    SET "Id_Entidad" = 9, "Ocupado" = 'SI', "ValorNicho" = 2500000, "Cuotas" = 12,
        "InteresMensual" = 2.5, "ConLapida" = 'NO', "ValorLapida" = NULL
    WHERE "Id_Nicho" = 2;

    FOR v_i IN 0..11 LOOP
        v_periodo := (DATE '2026-03-01' + make_interval(months => v_i))::date;
        v_vencimiento := (DATE '2026-04-10' + make_interval(months => v_i))::date;
        v_mov := NULL;

        IF v_i <= 5 THEN
            -- Marzo a agosto: pagadas unos días antes del vencimiento.
            INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                              "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
            VALUES (v_vencimiento - INTERVAL '3 days' + TIME '11:30', 3, 'INGRESO', 9, v_monto_cuota,
                    CASE WHEN v_i % 2 = 0 THEN 2 ELSE 1 END, CASE WHEN v_i % 2 = 0 THEN 2 ELSE 1 END, v_det_nicho,
                    'PAGO CUOTA NICHO ' || v_meses[EXTRACT(MONTH FROM v_periodo)::int] || ' ' || EXTRACT(YEAR FROM v_periodo))
            RETURNING "Id_Movimiento" INTO v_mov;
            v_estado := 'PAGADO';
        ELSIF v_vencimiento < DATE '2026-11-06' THEN
            v_estado := 'VENCIDA';
        ELSE
            v_estado := 'PENDIENTE';
        END IF;

        INSERT INTO cuotas ("Id_Entidad", "Mes_Periodo", "Anio_Periodo", "Id_TipoCuota", "Monto",
                            "FechaHoraGeneracion", "FechaVencimiento", "Estado", "Id_Movimiento")
        VALUES (9, EXTRACT(MONTH FROM v_periodo)::int, EXTRACT(YEAR FROM v_periodo)::int, v_tc_nicho, v_monto_cuota,
                v_generacion, v_vencimiento, v_estado, v_mov);
    END LOOP;
END $$;

COMMIT;

-- Control. En el informe totalizado del 01/01/2026 al 31/12/2026 deben aparecer:
--   INGRESOS / CUOTA NICHO:      6 movimientos, $ 1.281.250,02  (6 × 213.541,67)
--   EGRESOS  / PAGO A EMPLEADOS: 23 movimientos, $ 3.660.000,00 (10 × 180.000 + 10 × 150.000 + 3 × 120.000)
SELECT d."NombreDetalleMovimiento" AS actividad, UPPER(c."Tipo_Movimiento") AS tipo,
       COUNT(*) AS movimientos, SUM(c."Monto") AS total
FROM caja c
JOIN detalle_movimientos d ON d."Id_Detmov" = c."Id_Detmov"
WHERE d."NombreDetalleMovimiento" IN ('PAGO A EMPLEADOS', 'CUOTA NICHO')
GROUP BY 1, 2;

SELECT "Estado", COUNT(*) AS cuotas_nicho
FROM cuotas c JOIN tipo_cuotas t ON t."Id_TipoCuota" = c."Id_TipoCuota"
WHERE t."Concepto" = 'NICHO' AND c."Id_Entidad" = 9
GROUP BY 1;

-- Para deshacer estos datos de prueba (en este orden):
-- DELETE FROM cuotas WHERE "Id_Entidad" = 9 AND "Id_TipoCuota" = (SELECT "Id_TipoCuota" FROM tipo_cuotas WHERE "Concepto" = 'NICHO');
-- DELETE FROM caja WHERE "Id_Detmov" IN (SELECT "Id_Detmov" FROM detalle_movimientos WHERE "NombreDetalleMovimiento" IN ('PAGO A EMPLEADOS', 'CUOTA NICHO'));
-- UPDATE nichos SET "Id_Entidad" = NULL, "Ocupado" = 'NO', "ConLapida" = 'NO', "ValorLapida" = NULL,
--        "Cuotas" = NULL, "InteresMensual" = NULL WHERE "Id_Nicho" = 2;
-- (El concepto PAGO A EMPLEADOS no se borra: pertenece al catálogo y lo crea la migración.)
