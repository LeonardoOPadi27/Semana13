# Gimnasio API — Laboratorio Semana 13

Web API REST en capas (Entidades, Datos, Negocio y API) con ASP.NET Core (.NET 10), Entity Framework Core 10 y AutoMapper.
La API administra las **membresías** de un gimnasio y los **socios** inscritos en ellas, y solo expone DTOs.

## Modelo

| Entidad | Lado | Campos principales |
|---|---|---|
| `Membresia` | 1 | Nombre, Descripcion, PrecioMensual, DuracionMeses, Activo |
| `Socio` | N | Dni, Nombres, Apellidos, Email, Telefono, FechaNacimiento, FechaInscripcion, Activo, **IdMembresia (FK)** |

Una membresía tiene muchos socios. La base de datos se crea con [`Database/GimnasioDB.sql`](Database/GimnasioDB.sql) y el modelo se generó con Scaffold (base de datos primero).

## Arquitectura

```
Gimnasio.Api        → controladores y Program.cs          (referencia a Negocio)
Gimnasio.Negocio    → servicios, reglas, MappingProfile    (referencia a Datos)
Gimnasio.Datos      → GimnasioContext y repositorios       (referencia a Entidades)
Gimnasio.Entidades  → entidades y DTOs
```

## Reglas de negocio (409 Conflict)

1. No pueden existir dos membresías con el mismo nombre.
2. No se desactiva una membresía que tiene socios activos.
3. No se elimina una membresía que tiene socios inscritos.
4. El DNI del socio no se puede repetir.
5. El socio debe tener al menos 16 años.
6. Solo se inscribe a membresías que existen y están activas.

## Endpoints

| Método | Ruta | Códigos |
|---|---|---|
| GET | `api/membresias` | 200 |
| GET | `api/membresias/{id}` | 200, 404 |
| GET | `api/membresias/{id}/socios` (ruta anidada) | 200, 404 |
| POST | `api/membresias` | 201, 400, 409 |
| PUT | `api/membresias/{id}` | 204, 400, 404, 409 |
| DELETE | `api/membresias/{id}` | 204, 404, 409 |
| GET | `api/socios?nombre=&idMembresia=&activo=` (filtros) | 200 |
| GET | `api/socios/{id}` | 200, 404 |
| POST | `api/socios` | 201, 400, 409 |
| PUT | `api/socios/{id}` | 204, 400, 404, 409 |
| DELETE | `api/socios/{id}` | 204, 404 |

## Cómo ejecutarlo

1. Crear la base de datos ejecutando `Database/GimnasioDB.sql` (SQL Server o LocalDB).
2. Guardar la cadena de conexión con User Secrets (no se sube al repositorio):

   ```bash
   dotnet user-secrets set "ConnectionStrings:GimnasioDB" "<tu cadena de conexión a GimnasioDB>" --project Gimnasio.Api
   ```

3. Ejecutar la API:

   ```bash
   dotnet run --project Gimnasio.Api --launch-profile http
   ```

4. Abrir Swagger en `http://localhost:5091/swagger`.

## Pruebas

Las 30 peticiones (todos los endpoints y los códigos 200, 201, 204, 400, 404 y 409) están en
[`Gimnasio.Api/Gimnasio.Api.http`](Gimnasio.Api/Gimnasio.Api.http). Las capturas de cada prueba ejecutada en Swagger están en
[`Evidencias/Pruebas`](Evidencias/Pruebas).

## Semana 14 - Autenticacion JWT

La API conserva la arquitectura en capas de S13 y agrega autenticacion JWT. El registro publico siempre asigna el rol `Socio` en el servicio; el DTO no recibe un rol. El rol `Recepcionista` se reserva para personal del gimnasio y solo se asigna desde la base durante la preparacion de la cuenta.

### Base de datos y cuentas de prueba

`Database/GimnasioDB.sql` incluye la tabla `Usuario` y una cuenta BCrypt de prueba por rol. Si la base de S13 ya existe y se deben conservar sus datos, ejecutar `Database/Usuarios.sql`; es idempotente y solo agrega la tabla y las cuentas que falten. El script principal recrea `GimnasioDB`, por lo que no se debe volver a ejecutar sobre una base con datos que se quieran conservar.

| Rol | Correo | Clave de prueba |
|---|---|---|
| Socio | `socio.demo@gimnasio.test` | `SocioDemo2026!` |
| Recepcionista | `recepcionista.demo@gimnasio.test` | `RecepcionistaDemo2026!` |

Para demostrar que el servidor decide el rol: registra `recepcionista.s14@gimnasio.test` mediante `POST /api/auth/registro` y luego ejecuta `Database/PromoverRecepcionista.sql`. El registro le asigna primero `Socio`; el cambio autorizado se hace en SQL, no en el cuerpo HTTP.

### Configuracion segura

Issuer, Audience y expiracion estan en `Gimnasio.Api/appsettings.json`. La clave de firma no se guarda en el repositorio; se configura localmente con User Secrets:

```powershell
dotnet user-secrets set "Jwt:SecretKey" "<clave aleatoria de al menos 32 caracteres>" --project Gimnasio.Api
```

No se debe subir la clave JWT, una cadena de conexion ni un token funcional. La solucion requiere `BCrypt.Net-Next`, `System.IdentityModel.Tokens.Jwt` y `Microsoft.AspNetCore.Authentication.JwtBearer`.

### Matriz de permisos

| Endpoint | Sin token | Socio | Recepcionista |
|---|---|---|---|
| `POST /api/auth/registro` | Publico | Publico | Publico |
| `POST /api/auth/login` | Publico | Publico | Publico |
| `GET /api/auth/perfil` | 401 | 200 | 200 |
| `GET /api/membresias` y `GET /api/membresias/{id}` | 401 | 200 | 200 |
| `GET /api/membresias/{id}/socios` | 401 | 403 | 200 |
| `POST`, `PUT`, `DELETE /api/membresias` | 401 | 403 | 201 / 204 |
| `GET`, `POST`, `PUT`, `DELETE /api/socios` | 401 | 403 | 200 / 201 / 204 |

Las altas protegidas de membresias y socios requieren `Recepcionista`; las reglas de negocio y correo duplicado producen `409`. El perfil usa solo los claims del token.

### Sustentacion de seguridad

- `401` significa que falta una credencial valida; por ejemplo, abrir `/api/auth/perfil` sin token o iniciar sesion con correo o clave incorrectos. El login devuelve el mismo mensaje en ambos casos.
- `403` significa que el token es valido, pero el rol no autoriza la operacion; por ejemplo, un `Socio` intenta listar `/api/socios`.
- El JWT contiene `sub` (IdUsuario), `email` y `role`; no contiene la clave, el hash ni la clave de firma.
- Guardar claves en texto plano permitiria leerlas si se filtra la base. BCrypt conserva un hash con sal y permite verificar la clave sin almacenarla.
- El rol se asigna en el servicio de registro y se firma en el token. Aceptar el rol enviado por el cliente permitiria que cualquier persona se diera permisos de Recepcionista.

### Pruebas S14

Las peticiones de autenticacion y autorizacion se agregan al inicio de `Gimnasio.Api/Gimnasio.Api.http`. Ejecuta los requests `loginSocio` y `loginRecepcionista` para cargar las variables de token; las 30 peticiones S13 continuan con token de Recepcionista. El archivo contiene casos para 200, 201, 204, 400, 401, 403, 404 y 409.