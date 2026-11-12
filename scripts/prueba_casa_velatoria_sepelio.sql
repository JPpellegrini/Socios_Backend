-- Datos de PRUEBA para el listado de la casa velatoria (socios habilitados para sepelio).
-- Solo para la base local de desarrollo. No ejecutar en producción.
-- Requiere haber ejecutado antes scripts/prueba_socios_10.sql (los socios se ubican por DNI).
--
-- Carga cuotas SOCIO y SEPELIO 2026 (con su pago en caja cuando están pagadas) para los 5 socios
-- con sepelio de ese script, un caso distinto para cada condición del listado:
--
--   | Socio      | DNI      | Cuotas          | Pagadas hasta | Sepelio pagadas | Meses adeudados* | Resultado esperado          |
--   | GÓMEZ      | 20111222 | mayo a octubre  | octubre       | 6 (justo)       | 0                | FIGURA (mínimo exacto)      |
--   | SOSA       | 22444555 | enero a octubre | julio         | 7               | 3 (justo)        | FIGURA (máximo exacto)      |
--   | FERNÁNDEZ  | 16222333 | enero a octubre | junio         | 6               | 4                | NO figura: deuda            |
--   | BENÍTEZ    | 19777888 | junio a octubre | octubre       | 5               | 0                | NO figura: carencia (5 < 6) |
--   | CASTRO     | 14000111 | sep. y octubre  | octubre       | 2               | 0                | NO figura: carencia         |
--
--   * Las cuotas impagas quedan PENDIENTE (como en el sistema real) y cuentan como adeudadas cuando
--     pasa su vencimiento (día 10 del mes siguiente). La de octubre vence el 10/11/2026: los meses
--     adeudados indicados valen desde el 11/11/2026.
--
-- La cuota de sepelio usa el tipo que corresponde por plan y edad (tope 65, como la generación de
-- cuotas); todos estos socios superan los 65: plan A -> tipo 4, plan B -> tipo 5.
-- Si alguno de estos socios ya tiene cuotas, el script se detiene sin cargar nada.

BEGIN;

-- Las secuencias pueden haber quedado atrasadas por los datos semilla con Id explícito.
SELECT setval(pg_get_serial_sequence('caja', 'Id_Movimiento'), (SELECT MAX("Id_Movimiento") FROM caja));
SELECT setval(pg_get_serial_sequence('cuotas', 'Id_Deuda'), COALESCE((SELECT MAX("Id_Deuda") FROM cuotas), 1));

DO $$
DECLARE
    v_meses TEXT[] := ARRAY['ENE','FEB','MAR','ABR','MAY','JUN','JUL','AGO','SEP','OCT','NOV','DIC'];
    s RECORD;
    v_entidad INTEGER;
    v_plan TEXT;
    v_nacimiento TIMESTAMP;
    v_tc_sepelio INTEGER;
    v_importe_socio NUMERIC;
    v_importe_sepelio NUMERIC;
    v_m INTEGER;
    v_mov INTEGER;
    v_pagada BOOLEAN;
    v_fecha_pago TIMESTAMP;
    v_vencimiento DATE;
    v_metodo INTEGER;
BEGIN
    SELECT "Importe" INTO v_importe_socio FROM tipo_cuotas WHERE "Id_TipoCuota" = 1;

    FOR s IN
        SELECT * FROM (VALUES
            -- dni, primer mes, último mes pagado
            ('20111222', 5, 10),   -- GÓMEZ
            ('22444555', 1, 7),    -- SOSA
            ('16222333', 1, 6),    -- FERNÁNDEZ
            ('19777888', 6, 10),   -- BENÍTEZ
            ('14000111', 9, 10)    -- CASTRO
        ) AS t(dni, primer_mes, ultimo_pagado)
    LOOP
        SELECT e."Id_Entidad", so."Plan", e."Nacimiento" INTO v_entidad, v_plan, v_nacimiento
        FROM entidades e JOIN socios so ON so."Id_Entidad" = e."Id_Entidad"
        WHERE e."Dni" = s.dni;

        IF v_entidad IS NULL THEN
            RAISE EXCEPTION 'No existe el socio con DNI %: ejecute antes scripts/prueba_socios_10.sql.', s.dni;
        END IF;
        IF EXISTS (SELECT 1 FROM cuotas WHERE "Id_Entidad" = v_entidad) THEN
            RAISE EXCEPTION 'El socio con DNI % ya tiene cuotas: el script parece ya ejecutado.', s.dni;
        END IF;

        -- Tipo de cuota de sepelio por plan y edad (tope 65).
        IF v_nacimiento IS NOT NULL AND date_part('year', age(DATE '2026-01-01', v_nacimiento::date)) <= 65 THEN
            v_tc_sepelio := CASE WHEN v_plan = 'B' THEN 3 ELSE 2 END;
        ELSE
            v_tc_sepelio := CASE WHEN v_plan = 'B' THEN 5 ELSE 4 END;
        END IF;
        SELECT "Importe" INTO v_importe_sepelio FROM tipo_cuotas WHERE "Id_TipoCuota" = v_tc_sepelio;

        FOR v_m IN s.primer_mes..10 LOOP
            v_pagada := v_m <= s.ultimo_pagado;
            v_vencimiento := (make_date(2026, v_m, 1) + INTERVAL '1 month' + INTERVAL '9 days')::date;  -- día 10 del mes siguiente
            v_fecha_pago := make_date(2026, v_m, 1) + INTERVAL '1 month' + INTERVAL '4 days' + TIME '10:30';  -- día 5 del mes siguiente
            v_metodo := CASE WHEN v_m % 2 = 0 THEN 1 ELSE 2 END;  -- 1 efectivo / 2 transferencia

            -- Cuota SOCIO
            v_mov := NULL;
            IF v_pagada THEN
                INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                                  "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
                VALUES (v_fecha_pago, 1, 'INGRESO', v_entidad, v_importe_socio, v_metodo, v_metodo, 1,
                        'PAGO CUOTA SOCIO ' || v_meses[v_m] || ' 2026')
                RETURNING "Id_Movimiento" INTO v_mov;
            END IF;
            INSERT INTO cuotas ("Id_Entidad", "Mes_Periodo", "Anio_Periodo", "Id_TipoCuota", "Monto",
                                "FechaHoraGeneracion", "FechaVencimiento", "Estado", "Id_Movimiento")
            VALUES (v_entidad, v_m, 2026, 1, v_importe_socio, make_date(2026, v_m, 1), v_vencimiento,
                    CASE WHEN v_pagada THEN 'PAGADO' ELSE 'PENDIENTE' END, v_mov);

            -- Cuota SEPELIO
            v_mov := NULL;
            IF v_pagada THEN
                INSERT INTO caja ("FechaHoraMov", "Id_CajaDiaria", "Tipo_Movimiento", "Id_Entidad", "Monto",
                                  "Id_MetodoPago", "Id_Cuenta", "Id_Detmov", "Observacion")
                VALUES (v_fecha_pago + INTERVAL '1 minute', 1, 'INGRESO', v_entidad, v_importe_sepelio, v_metodo, v_metodo, 2,
                        'PAGO CUOTA SEPELIO ' || v_meses[v_m] || ' 2026')
                RETURNING "Id_Movimiento" INTO v_mov;
            END IF;
            INSERT INTO cuotas ("Id_Entidad", "Mes_Periodo", "Anio_Periodo", "Id_TipoCuota", "Monto",
                                "FechaHoraGeneracion", "FechaVencimiento", "Estado", "Id_Movimiento")
            VALUES (v_entidad, v_m, 2026, v_tc_sepelio, v_importe_sepelio, make_date(2026, v_m, 1), v_vencimiento,
                    CASE WHEN v_pagada THEN 'PAGADO' ELSE 'PENDIENTE' END, v_mov);
        END LOOP;
    END LOOP;
END $$;

COMMIT;

-- Control: cuotas de sepelio pagadas y meses adeudados de cada socio a hoy.
SELECT e."Apellido",
       (SELECT COUNT(*) FROM cuotas c JOIN tipo_cuotas t ON t."Id_TipoCuota" = c."Id_TipoCuota"
         WHERE c."Id_Entidad" = e."Id_Entidad" AND c."Estado" = 'PAGADO' AND t."Concepto" = 'SEPELIO') AS sepelio_pagadas,
       (SELECT COUNT(DISTINCT c."Anio_Periodo" * 100 + c."Mes_Periodo") FROM cuotas c
         WHERE c."Id_Entidad" = e."Id_Entidad"
           AND (c."Estado" = 'VENCIDA' OR (c."Estado" = 'PENDIENTE' AND c."FechaVencimiento" < CURRENT_DATE))) AS meses_adeudados
FROM entidades e
WHERE e."Dni" IN ('20111222','22444555','16222333','19777888','14000111')
ORDER BY e."Apellido";

-- Para borrar estos datos de prueba (primero las cuotas, que referencian a caja):
-- DELETE FROM cuotas WHERE "Id_Entidad" IN (SELECT "Id_Entidad" FROM entidades WHERE "Dni" IN ('20111222','22444555','16222333','19777888','14000111'));
-- DELETE FROM caja   WHERE "Id_Entidad" IN (SELECT "Id_Entidad" FROM entidades WHERE "Dni" IN ('20111222','22444555','16222333','19777888','14000111'));
