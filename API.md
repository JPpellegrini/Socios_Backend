# API Socios — Guía rápida

## Conexión

- **Base URL (dev):** `http://localhost:5056`
- **Staging URL (dev):** `http://cjr-staging.runasp.net`
- **Autenticación:** Basic Auth en **todos** los endpoints. Se manda el header
  `Authorization: Basic <base64("usuario:contraseña")>`.
  - Usuarios de prueba (seed): `CJR` — contraseña `1234`.
- Si falta el header o las credenciales son inválidas → **401 Unauthorized**.

---

## 1) GET `/api/v1/me` — Usuario autenticado

Devuelve los datos del usuario logueado. No recibe body.

**Respuesta 200:**
```json
{ 
  "id_Usuario": 1, 
  "usuarioNombre": "CJR", 
  "estado": "Activo", 
  "id_Rol": 1, 
  "rolNombre": "Secretaria" 
}
```

**Errores:** 
`401` sin credenciales.
`404` si el usuario no existe.

---

## 2) GET `/api/v1/socios` — Listado de socios

Devuelve lista de socios.

**Body**
**Filtros (todos opcionales):**
- `busqueda`: parte del nombre o apellido.
- `dni`: parte del DNI.
- `incluirInactivos`: `false` (default) solo activos · `true` incluye inactivos.

**Respuesta 200:**
```json
[ 
  { 
    "idSocio": 4, 
    "nombre": "María", 
    "apellido": "González", 
    "dni": "30111222", 
    "estado": "ACTIVO" 
  } ,
  {
    ...
  }
]
```

**Errores:** 
`401` sin credenciales.

---

## 3) GET `/api/v1/buscarentidad` — Buscar entidad por DNI

Busca una entidad (persona) por DNI exacto. Sirve para autocompletar datos antes de dar de alta,
sin tener que recordar todos los datos.


**Body**
- `dni` requerido, 7-8 dígitos.

**Respuesta 200:**
```json
{
  "id_Entidad": 1, 
  "cuitCuil": "20321270574", 
  "nombre": "Luciano", 
  "apellido": "Oldan",
  "razonSocial": null, 
  "sexo": "Hombre", 
  "nacimiento": "1959-07-12",
  "ciudad": { 
    "id_Ciudad": 1, 
    "nombre": "Roldán" 
  },
  "calle": "Independencia", 
  "altura": 250, 
  "observacion": null
}
```

**Errores:** 
`404` si no encuentra la entidad.
`400` si el DNI tiene formato inválido.
`401` sin credenciales.

---

## 4) POST `/api/v1/socios/crear` — Alta de socio

Crea el socio y, en cascada, su entidad, contactos y estado.

**Respuesta 200**
```json
{ 
  "idSocio": 4 
}
```

**Errores 400 (validación):** ejemplo de cuerpo de error:
```json
{
  "status": 400,
  "errors": {
    "Nombre": ["El nombre es obligatorio."],
    "Telefonos": ["Debe cargar al menos un teléfono."],
    "Dni": ["El DNI debe tener 7 u 8 dígitos."]
  }
}
```

**Errores:**
`401` sin credenciales.
`401`: 
- falta un campo requerido.
- DNI/teléfono/email con formato inválido.
- `plan`/`sepelio`/`cobrador` con un valor no permitido. 

---

## Resumen de errores comunes

| Código | Significado |
|--------|-------------|
| `200`  | OK          |
| `400`  | Datos inválidos (ver `errors` en el cuerpo) |
| `401`  | Falta el header `Authorization` o credenciales incorrectas |
| `404`  | No se encontró el recurso (ej: entidad por DNI) |
| `500`  | Error interno del servidor |
