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
