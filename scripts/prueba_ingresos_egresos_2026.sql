-- Datos de PRUEBA masivos para el reporte de ingresos y egresos (enero a octubre de 2026).
-- Solo para la base local de desarrollo. No ejecutar en producción.
--
-- Qué carga:
--   1) Cuotas SOCIO (y SEPELIO si el socio tiene sepelio) de enero a octubre 2026 para los socios
--      ACTIVOS que todavía no tienen cuotas (excluye a las entidades 1 y 5, que tienen sus propios
--      datos de prueba). La mayoría quedan PAGADAS con su ingreso en caja vinculado; algunas quedan
--      VENCIDAS (hasta agosto) o PENDIENTES (septiembre y octubre). Así también sirven para el
--      estado de cuenta de cada socio.
--   2) Egresos mensuales a los proveedores 1002, 1003 y 1004 (este último, inactivo, solo hasta marzo).
--   3) Ingresos mensuales por colaboración de los colaboradores 18 y 19.
--   4) Algunos ajustes de diferencia de caja (ingresos y egresos chicos).
--
-- Es determinístico (no usa azar): los totales de control del final siempre dan lo mismo.
-- Si alguna de esas entidades ya tiene movimientos, el script se detiene sin cargar nada.

BEGIN;

-- Las secuencias pueden haber quedado atrasadas por los datos semilla con Id explícito.
SELECT setval(pg_get_serial_sequence('caja', 'Id_Movimiento'), (SELECT MAX("Id_Movimiento") FROM caja));
SELECT setval(pg_get_serial_sequence('cuotas', 'Id_Deuda'), COALESCE((SELECT MAX("Id_Deuda") FROM cuotas), 1));

DO $$
DECLARE
    v_meses TEXT[] := ARRAY['ENE','FEB','MAR','ABR','MAY','JUN','JUL','AGO','SEP','OCT','NOV','DIC'];
    s RECORD;
    v_i INTEGER := 0;
    v_m INTEGER;
    v_mov INTEGER;
    v_tc_sepelio INTEGER;
    v_importe_socio NUMERIC;
    v_importe_sepelio NUMERIC;
    v_pagada BOOLEAN;
    v_metodo INTEGER;
    v_fecha_pago TIMESTAMP;
    v_vencimiento DATE;
BEGIN
    -- Protección: no duplicar si el script ya se ejecutó.
    IF EXISTS (SELECT 1 FROM caja WHERE "Id_Entidad" IN (1002, 1003, 1004, 18, 19)) THEN
        RAISE EXCEPTION 'Los proveedores 1002-1004 o los colaboradores 18-19 ya tienen movimientos: el script parece ya ejecutado.';
    END IF;

    SELECT "Importe" INTO v_importe_socio FROM tipo_cuotas WHERE "Id_TipoCuota" = 1;

    ---------------------------------------------------------------------------------------------
    -- 1) Cuotas de socios activos sin cuotas previas.
    ---------------------------------------------------------------------------------------------
    FOR s IN
        SELECT so."Id_Entidad", so."Plan", so."Sepelio", e."Nacimiento"
        FROM socios so
        JOIN entidades e ON e."Id_Entidad" = so."Id_Entidad"
        WHERE so."Id_Entidad" NOT IN (1, 5)
          AND EXISTS (SELECT 1 FROM entidad_tipos et
                      WHERE et."Id_Entidad" = so."Id_Entidad" AND et."Id_Tipo" = 1 AND et."Estado" = 'ACTIVO')
          AND NOT EXISTS (SELECT 1 FROM cuotas c WHERE c."Id_Entidad" = so."Id_Entidad")
        ORDER BY so."Id_Entidad"
    LOOP
        v_i := v_i + 1;

        -- Tipo de cuota de sepelio según plan y edad (tope 65, igual que la generación de cuotas):
        -- plan A: 2 (hasta 65) / 4 (más de 65); plan B: 3 (hasta 65) / 5 (más de 65).
        v_tc_sepelio := NULL;
        IF s."Sepelio" = 'SI' THEN
            IF s."Nacimiento" IS NOT NULL AND date_part('year', age(DATE '2026-01-01', s."Nacimiento"::date)) <= 65 THEN
                v_tc_sepelio := CASE WHEN s."Plan" = 'B' THEN 3 ELSE 2 END;
            ELSE
                v_tc_sepelio := CASE WHEN s."Plan" = 'B' THEN 5 ELSE 4 END;
            END IF;
            SELECT "Importe" INTO v_importe_sepelio FROM tipo_cuotas WHERE "Id_TipoCuota" = v_tc_sepelio;
        END IF;

        FOR v_m IN 1..10 LOOP
            v_vencimiento := (make_date(2026, v_m, 1) + INTERVAL '1 month' + INTERVAL '9 days')::date;
            -- Octubre queda siempre pendiente (vence el 10/11); del resto, una de cada 7 queda impaga.
            v_pagada := v_m < 10 AND (v_m + v_i) % 7 <> 0;
            v_metodo := CASE WHEN (v_m + v_i) % 2 = 0 THEN 1 ELSE 2 END;   -- 1 efectivo / 2 transferencia
            v_fecha_pago := make_date(2026, v_m, 1) + INTERVAL '1 month'
                            + make_interval(days => (v_i % 5) + 2, hours => 9 + (v_i % 6), mins => v_m * 3);

            -- Cuota SOCIO
            v_mov := NULL;
            IF v_pagada THEN
                INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                                  "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
                VALUES (v_fecha_pago, 1, 'INGRESO', s."Id_Entidad", v_importe_socio, v_metodo, v_metodo, 1,
                        'PAGO CUOTA SOCIO ' || v_meses[v_m] || ' 2026')
                RETURNING "Id_Movimiento" INTO v_mov;
            END IF;
            INSERT INTO cuotas ("Id_Entidad", "Mes_Periodo", "Anio_Periodo", "Id_TipoCuota", "Monto",
                                "FechaHoraGeneracion", "FechaVencimiento", "Estado", "Id_Movimiento")
            VALUES (s."Id_Entidad", v_m, 2026, 1, v_importe_socio, make_date(2026, v_m, 1), v_vencimiento,
                    CASE WHEN v_pagada THEN 'PAGADO' WHEN v_m <= 8 THEN 'VENCIDA' ELSE 'PENDIENTE' END, v_mov);

            -- Cuota SEPELIO (si corresponde), mismo criterio de pago y mismo día, un minuto después.
            IF v_tc_sepelio IS NOT NULL THEN
                v_mov := NULL;
                IF v_pagada THEN
                    INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                                      "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
                    VALUES (v_fecha_pago + INTERVAL '1 minute', 1, 'INGRESO', s."Id_Entidad", v_importe_sepelio,
                            v_metodo, v_metodo, 2, 'PAGO CUOTA SEPELIO ' || v_meses[v_m] || ' 2026')
                    RETURNING "Id_Movimiento" INTO v_mov;
                END IF;
                INSERT INTO cuotas ("Id_Entidad", "Mes_Periodo", "Anio_Periodo", "Id_TipoCuota", "Monto",
                                    "FechaHoraGeneracion", "FechaVencimiento", "Estado", "Id_Movimiento")
                VALUES (s."Id_Entidad", v_m, 2026, v_tc_sepelio, v_importe_sepelio, make_date(2026, v_m, 1), v_vencimiento,
                        CASE WHEN v_pagada THEN 'PAGADO' WHEN v_m <= 8 THEN 'VENCIDA' ELSE 'PENDIENTE' END, v_mov);
            END IF;
        END LOOP;
    END LOOP;

    ---------------------------------------------------------------------------------------------
    -- 2) Proveedores (EGRESO, concepto 5 PAGO A PROVEEDOR).
    ---------------------------------------------------------------------------------------------
    FOR v_m IN 1..10 LOOP
        -- 1002 Insumos Médicos Rosario: factura mensual creciente, por transferencia, día 15.
        INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                          "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
        VALUES (make_date(2026, v_m, 15) + TIME '11:00', 3, 'EGRESO', 1002, 85000 + v_m * 1500, 2, 2, 5,
                'FACTURA INSUMOS ' || v_meses[v_m] || ' 2026');

        -- 1003 Servicios Integrales Funes: alquiler de salón fijo, en efectivo, día 3.
        INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                          "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
        VALUES (make_date(2026, v_m, 3) + TIME '09:30', 1, 'EGRESO', 1003, 60000, 1, 1, 5,
                'ALQUILER SALÓN ' || v_meses[v_m] || ' 2026');

        -- 1004 Mantenimiento Roldán (inactivo): solo de enero a marzo.
        IF v_m <= 3 THEN
            INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                              "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
            VALUES (make_date(2026, v_m, 25) + TIME '15:45', 3, 'EGRESO', 1004, 45000, 1, 1, 5,
                    'MANTENIMIENTO ' || v_meses[v_m] || ' 2026');
        END IF;
    END LOOP;

    ---------------------------------------------------------------------------------------------
    -- 3) Colaboradores (INGRESO, concepto 4 COLABORACIÓN).
    ---------------------------------------------------------------------------------------------
    FOR v_m IN 1..10 LOOP
        -- 18 PÉREZ, RAÚL (podología): mensual, día 20.
        INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                          "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
        VALUES (make_date(2026, v_m, 20) + TIME '17:10', 3, 'INGRESO', 18, 25000, 1, 1, 4,
                'COLABORACIÓN PODOLOGÍA ' || v_meses[v_m] || ' 2026');

        -- 19 MOLINA, ANDREA LAURA (cardiología): meses pares, por transferencia, día 12.
        IF v_m % 2 = 0 THEN
            INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                              "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
            VALUES (make_date(2026, v_m, 12) + TIME '10:20', 1, 'INGRESO', 19, 40000, 2, 2, 4,
                    'COLABORACIÓN CARDIOLOGÍA ' || v_meses[v_m] || ' 2026');
        END IF;
    END LOOP;

    -- Devolución a un colaborador (egreso en colaboración).
    INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                      "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
    VALUES ('2026-06-22 12:00', 1, 'EGRESO', 18, 5000, 1, 1, 4, 'DEVOLUCIÓN COLABORACIÓN JUNIO');

    ---------------------------------------------------------------------------------------------
    -- 4) Ajustes de diferencia de caja (concepto 3), imputados al proveedor 1003.
    ---------------------------------------------------------------------------------------------
    INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                      "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion") VALUES
    ('2026-03-31 18:00', 1, 'EGRESO',  1003,  850.00, 1, 1, 3, 'AJUSTE FALTANTE CIERRE MARZO'),
    ('2026-07-31 18:00', 1, 'INGRESO', 1003,  420.50, 1, 1, 3, 'AJUSTE SOBRANTE CIERRE JULIO');
END $$;

COMMIT;

-- Resultado esperado en el reporte con el período 01/01/2026 al 31/12/2026 (verificado con la
-- base local, que ya tenía 3 movimientos semilla en junio):
--   Solo con este script:            98 movimientos | ingresos $ 796.920,50   | egresos $ 1.715.850,47
--   Con los otros 3 scripts de prueba: 129 movimientos | ingresos $ 1.106.420,50 | egresos $ 3.158.349,47
--   Cuotas cargadas por este script: 54 PAGADAS, 8 VENCIDAS, 8 PENDIENTES.
--
-- Control: movimientos de 2026 por concepto y tipo (incluye los de otros scripts que se hayan ejecutado).
SELECT d."NombreDetalleMovimiento" AS actividad,
       UPPER(c."Tipo_Movimiento")  AS tipo,
       COUNT(*)                    AS movimientos,
       SUM(c."Monto")              AS total
FROM caja c
JOIN detalle_movimientos d ON d."Id_Detmov" = c."Id_Detmov"
WHERE c."FechaHoraMov" >= '2026-01-01' AND c."FechaHoraMov" < '2027-01-01'
GROUP BY d."NombreDetalleMovimiento", UPPER(c."Tipo_Movimiento")
ORDER BY 1, 2;

-- Para borrar estos datos de prueba (primero las cuotas, que referencian a caja):
-- DELETE FROM cuotas WHERE "Anio_Periodo" = 2026 AND "Id_Entidad" NOT IN (1, 5)
--   AND "Id_Entidad" IN (SELECT "Id_Entidad" FROM socios);
-- DELETE FROM caja WHERE "FechaHoraMov" >= '2026-01-01' AND "Id_Entidad" NOT IN (1, 4, 5, 1001, 3)
--   AND ("Observacion" LIKE 'PAGO CUOTA % 2026' OR "Id_Entidad" IN (1002, 1003, 1004, 18, 19));
