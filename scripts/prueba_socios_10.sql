-- Datos de PRUEBA: 10 socios más, para el informe de socios (y el de altas y bajas).
-- Solo para la base local de desarrollo. No ejecutar en producción.
--
-- Cada socio se carga como lo hace el alta del sistema (SocioUseCase.CrearAsync):
--   entidades (persona, DNI) + socios + entidad_tipos (tipo 1 Socio) + contactos (TELEFONO / MAIL).
-- Los INACTIVOS además tienen su registro en entidad_bajas (como la baja del sistema).
--
-- Variedad para probar filtros y columnas del informe:
--   | Socio                     | Edad* | Ciudad  | Plan | Sepelio | Obra social | Estado    | Alta       | Contactos   |
--   | GÓMEZ, CARLOS ALBERTO     |  76   | Roldán  |  A   | SI      | PAMI        | ACTIVO    | 2015-03-10 | tel + mail  |
--   | FERNÁNDEZ, MARTA SUSANA   |  78   | Funes   |  B   | SI      | PAMI        | ACTIVO    | 2018-07-22 | tel         |
--   | RUIZ, JORGE OMAR          |  64   | Rosario |  A   | NO      | OSDE        | ACTIVO    | 2021-11-05 | tel + mail  |
--   | SOSA, ELENA BEATRIZ       |  71   | Roldán  |  B   | SI      | (sin O.S.)  | ACTIVO    | 2019-04-18 | tel         |
--   | MARTÍNEZ, RAÚL HÉCTOR     |  82   | Roldán  |  A   | SI      | PAMI        | INACTIVO  | 2010-09-01 | tel         |  baja 12/05/2026 FALLECIMIENTO
--   | LUNA, GRACIELA NOEMÍ      |  68   | Funes   |  B   | NO      | PAMI        | INACTIVO  | 2026-02-14 | tel + mail  |  baja 03/08/2026 RENUNCIA
--   | BENÍTEZ, OSVALDO RUBÉN    |  73   | Rosario |  A   | SI      | OSDE        | ACTIVO    | 2026-06-09 | (ninguno)   |
--   | ACOSTA, NORMA ISABEL      |  77   | Roldán  |  A   | NO      | PAMI        | ACTIVO    | 2026-10-02 | tel + mail  |
--   | PERALTA, HUGO DANIEL      |  66   | Roldán  |  B   | SI      | PAMI        | INACTIVO  | 2017-01-30 | tel         |  baja 20/03/2026 MORA
--   | CASTRO, ROSA AMELIA       |  79   | Funes   |  A   | SI      | (sin O.S.)  | ACTIVO    | 2026-09-15 | tel         |
--   * Edad al 06/11/2026.
--
-- Si alguno de estos DNI ya existe, el script se detiene sin cargar nada.

BEGIN;

-- Las secuencias pueden haber quedado atrasadas por los datos semilla con Id explícito.
SELECT setval(pg_get_serial_sequence('entidades', 'Id_Entidad'), (SELECT MAX("Id_Entidad") FROM entidades));
SELECT setval(pg_get_serial_sequence('socios', 'Id_Socio'), (SELECT MAX("Id_Socio") FROM socios));
SELECT setval(pg_get_serial_sequence('entidad_tipos', 'Id_EntidadTipo'), (SELECT MAX("Id_EntidadTipo") FROM entidad_tipos));
SELECT setval(pg_get_serial_sequence('contactos', 'Id_Contacto'), (SELECT MAX("Id_Contacto") FROM contactos));
SELECT setval(pg_get_serial_sequence('entidad_bajas', 'Id_Baja'), COALESCE((SELECT MAX("Id_Baja") FROM entidad_bajas), 1));

DO $$
DECLARE
    s RECORD;
    v_entidad INTEGER;
    v_entidad_tipo INTEGER;
BEGIN
    IF EXISTS (SELECT 1 FROM entidades WHERE "Dni" IN
        ('20111222','16222333','26333444','22444555','12555666','24666777','19777888','17888999','23999000','14000111')) THEN
        RAISE EXCEPTION 'Alguno de los DNI de prueba ya existe: el script parece ya ejecutado.';
    END IF;

    FOR s IN
        SELECT * FROM (VALUES
          -- dni, apellido, nombre, sexo, nacimiento, ciudad, calle, altura, plan, sepelio, cobrador, obra social, n° afiliado,
          -- fecha alta, estado, fecha baja, motivo baja, teléfono, mail
          ('20111222','GÓMEZ','CARLOS ALBERTO','HOMBRE',DATE '1950-05-14',1,'SAN MARTÍN',845,'A','SI','SI',1,'150111222330/00',
           DATE '2015-03-10','ACTIVO',NULL::date,NULL,'3414101010','carlos.gomez@mail.com'),
          ('16222333','FERNÁNDEZ','MARTA SUSANA','MUJER',DATE '1948-02-03',2,'CÓRDOBA',1520,'B','SI','NO',1,'150222333440/01',
           DATE '2018-07-22','ACTIVO',NULL,NULL,'3414202020',NULL),
          ('26333444','RUIZ','JORGE OMAR','HOMBRE',DATE '1962-09-27',3,'PELLEGRINI',2310,'A','NO','NO',2,'OSDE-4410287',
           DATE '2021-11-05','ACTIVO',NULL,NULL,'3416303030','jorge.ruiz@mail.com'),
          ('22444555','SOSA','ELENA BEATRIZ','MUJER',DATE '1955-06-08',1,'BELGRANO',312,'B','SI','SI',NULL,NULL,
           DATE '2019-04-18','ACTIVO',NULL,NULL,'3414404040',NULL),
          ('12555666','MARTÍNEZ','RAÚL HÉCTOR','HOMBRE',DATE '1944-01-19',1,'MORENO',77,'A','SI','NO',1,'150555666770/00',
           DATE '2010-09-01','INACTIVO',DATE '2026-05-12','FALLECIMIENTO','3414505050',NULL),
          ('24666777','LUNA','GRACIELA NOEMÍ','MUJER',DATE '1958-03-30',2,'SARMIENTO',940,'B','NO','NO',1,'150666777880/02',
           DATE '2026-02-14','INACTIVO',DATE '2026-08-03','RENUNCIA','3414606060','graciela.luna@mail.com'),
          ('19777888','BENÍTEZ','OSVALDO RUBÉN','HOMBRE',DATE '1952-11-11',3,'OROÑO',1655,'A','SI','NO',2,'OSDE-5520931',
           DATE '2026-06-09','ACTIVO',NULL,NULL,NULL,NULL),
          ('17888999','ACOSTA','NORMA ISABEL','MUJER',DATE '1949-08-25',1,'ITALIA',230,'A','NO','SI',1,'150888999000/00',
           DATE '2026-10-02','ACTIVO',NULL,NULL,'3414808080','norma.acosta@mail.com'),
          ('23999000','PERALTA','HUGO DANIEL','HOMBRE',DATE '1960-04-02',1,'URQUIZA',1108,'B','SI','NO',1,'150999000110/00',
           DATE '2017-01-30','INACTIVO',DATE '2026-03-20','MORA','3414909090',NULL),
          ('14000111','CASTRO','ROSA AMELIA','MUJER',DATE '1946-12-24',2,'LIBERTAD',58,'A','SI','NO',NULL,NULL,
           DATE '2026-09-15','ACTIVO',NULL,NULL,'3415010101',NULL)
        ) AS t(dni, apellido, nombre, sexo, nacimiento, id_ciudad, calle, altura, plan, sepelio, cobrador, id_os, afiliado,
               fecha_alta, estado, fecha_baja, motivo_baja, telefono, mail)
    LOOP
        INSERT INTO entidades ("Tipo", "Dni", "Nombre", "Apellido", "Sexo", "Nacimiento", "Id_Ciudad", "Calle", "Altura")
        VALUES ('DNI', s.dni, s.nombre, s.apellido, s.sexo, s.nacimiento, s.id_ciudad, s.calle, s.altura)
        RETURNING "Id_Entidad" INTO v_entidad;

        INSERT INTO socios ("Id_Entidad", "Id_OS", "Plan", "Sepelio", "Cobrador", "Numero_Afiliado")
        VALUES (v_entidad, s.id_os, s.plan, s.sepelio, s.cobrador, s.afiliado);

        INSERT INTO entidad_tipos ("Id_Entidad", "Id_Tipo", "Fecha_Alta", "Estado")
        VALUES (v_entidad, 1, s.fecha_alta, s.estado)
        RETURNING "Id_EntidadTipo" INTO v_entidad_tipo;

        IF s.telefono IS NOT NULL THEN
            INSERT INTO contactos ("Id_Entidad", "Tipo", "ContactoEntidad") VALUES (v_entidad, 'TELEFONO', s.telefono);
        END IF;
        IF s.mail IS NOT NULL THEN
            INSERT INTO contactos ("Id_Entidad", "Tipo", "ContactoEntidad") VALUES (v_entidad, 'MAIL', s.mail);
        END IF;

        IF s.fecha_baja IS NOT NULL THEN
            INSERT INTO entidad_bajas ("Id_EntidadTipo", "Fecha_Baja", "Motivo")
            VALUES (v_entidad_tipo, s.fecha_baja, s.motivo_baja);
        END IF;
    END LOOP;
END $$;

COMMIT;

-- Control: los 10 socios cargados.
SELECT so."Id_Socio", e."Id_Entidad", e."Apellido", e."Nombre", e."Dni", so."Plan", so."Sepelio",
       et."Estado", et."Fecha_Alta", eb."Fecha_Baja", eb."Motivo"
FROM entidades e
JOIN socios so ON so."Id_Entidad" = e."Id_Entidad"
JOIN entidad_tipos et ON et."Id_Entidad" = e."Id_Entidad" AND et."Id_Tipo" = 1
LEFT JOIN entidad_bajas eb ON eb."Id_EntidadTipo" = et."Id_EntidadTipo"
WHERE e."Dni" IN ('20111222','16222333','26333444','22444555','12555666','24666777','19777888','17888999','23999000','14000111')
ORDER BY e."Apellido";

-- Para borrar estos datos de prueba (en este orden):
-- DELETE FROM entidad_bajas WHERE "Id_EntidadTipo" IN (SELECT et."Id_EntidadTipo" FROM entidad_tipos et JOIN entidades e ON e."Id_Entidad" = et."Id_Entidad"
--   WHERE e."Dni" IN ('20111222','16222333','26333444','22444555','12555666','24666777','19777888','17888999','23999000','14000111'));
-- DELETE FROM contactos     WHERE "Id_Entidad" IN (SELECT "Id_Entidad" FROM entidades WHERE "Dni" IN ('20111222','16222333','26333444','22444555','12555666','24666777','19777888','17888999','23999000','14000111'));
-- DELETE FROM entidad_tipos WHERE "Id_Entidad" IN (SELECT "Id_Entidad" FROM entidades WHERE "Dni" IN ('20111222','16222333','26333444','22444555','12555666','24666777','19777888','17888999','23999000','14000111'));
-- DELETE FROM socios        WHERE "Id_Entidad" IN (SELECT "Id_Entidad" FROM entidades WHERE "Dni" IN ('20111222','16222333','26333444','22444555','12555666','24666777','19777888','17888999','23999000','14000111'));
-- DELETE FROM entidades     WHERE "Dni" IN ('20111222','16222333','26333444','22444555','12555666','24666777','19777888','17888999','23999000','14000111');
