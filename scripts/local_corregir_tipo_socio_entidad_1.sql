-- Corrige el tipo de entidad de OLDAN, LUCIANO (entidad 1). SOLO base local de desarrollo.
--
-- Problema: en la semilla (SociosDbContext) la fila 1 de entidad_tipos es Socio (tipo 1) INACTIVO,
-- y la baja 1 (MORA, 18/01/2016) apunta a esa fila. En esta base la fila 1 se modificó a mano a
-- Empleado (tipo 4) ACTIVO: Oldan dejó de figurar como socio (no aparece en el informe de socios)
-- y su baja por mora quedó colgada de una fila de empleado activo.
--
-- Corrección:
--   1) La fila 1 vuelve a lo que define la semilla: Socio INACTIVO (coherente con su baja por mora).
--   2) Se conserva el rol de Empleado ACTIVO en una fila nueva, con la misma fecha de alta.
-- Las condiciones evitan tocar algo si la base no está en el estado descripto.

BEGIN;

SELECT setval(pg_get_serial_sequence('entidad_tipos', 'Id_EntidadTipo'), (SELECT MAX("Id_EntidadTipo") FROM entidad_tipos));

INSERT INTO entidad_tipos ("Id_Entidad", "Id_Tipo", "Fecha_Alta", "Estado")
SELECT 1, 4, et."Fecha_Alta", 'ACTIVO'
FROM entidad_tipos et
WHERE et."Id_EntidadTipo" = 1 AND et."Id_Entidad" = 1 AND et."Id_Tipo" = 4
  AND NOT EXISTS (SELECT 1 FROM entidad_tipos x WHERE x."Id_Entidad" = 1 AND x."Id_Tipo" = 4 AND x."Id_EntidadTipo" <> 1);

UPDATE entidad_tipos
SET "Id_Tipo" = 1, "Estado" = 'INACTIVO'
WHERE "Id_EntidadTipo" = 1 AND "Id_Entidad" = 1 AND "Id_Tipo" = 4;

-- Control: debe mostrar la fila 1 como tipo 1 INACTIVO (con su baja MORA) y otra fila tipo 4 ACTIVO.
SELECT et."Id_EntidadTipo", et."Id_Tipo", et."Estado", et."Fecha_Alta", eb."Motivo" AS baja
FROM entidad_tipos et
LEFT JOIN entidad_bajas eb ON eb."Id_EntidadTipo" = et."Id_EntidadTipo"
WHERE et."Id_Entidad" = 1
ORDER BY 1;

COMMIT;
