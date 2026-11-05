-- Quita el tipo COLABORADOR de la entidad 1 (OLDAN, LUCIANO). SOLO base local de desarrollo.
--
-- Motivo: la semilla de la migración inicial (DBCompleta) le asigna el tipo colaborador
-- (fila Id_EntidadTipo = 2, Id_Tipo = 3) pero no crea su fila en "colaboradores", por lo que
-- aparece como colaborador sin prestación en el reporte de movimientos por entidad.
-- La entidad no tiene movimientos de COLABORACIÓN, así que no se pierde ningún dato real.
--
-- Pendiente: corregir la semilla al limpiar los datos de prueba de las migraciones; si no,
-- la fila vuelve a aparecer en cualquier base creada desde cero.

BEGIN;

-- Control previo: debe mostrar una fila (Id_EntidadTipo 2, entidad 1, tipo 3).
SELECT "Id_EntidadTipo", "Id_Entidad", "Id_Tipo", "Estado"
FROM entidad_tipos
WHERE "Id_EntidadTipo" = 2;

-- entidad_bajas referencia a entidad_tipos: si hubiera una baja registrada para esta fila,
-- el DELETE fallaría. Debe dar 0.
SELECT COUNT(*) AS bajas_asociadas
FROM entidad_bajas
WHERE "Id_EntidadTipo" = 2;

-- Las condiciones extra evitan borrar otra fila si en esta base el Id 2 fuera otra cosa.
DELETE FROM entidad_tipos
WHERE "Id_EntidadTipo" = 2
  AND "Id_Entidad" = 1
  AND "Id_Tipo" = 3
  AND NOT EXISTS (SELECT 1 FROM colaboradores c WHERE c."Id_Entidad" = 1);

-- Control posterior: debe dar 0.
SELECT COUNT(*) AS tipos_colaborador_entidad_1
FROM entidad_tipos
WHERE "Id_Entidad" = 1 AND "Id_Tipo" = 3;

COMMIT;
