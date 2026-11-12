using Gimnasio.Datos.Contexto;
using Gimnasio.Datos.Repositorios;
using Gimnasio.Negocio.Mapeos;
using Gimnasio.Negocio.Servicios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Base de datos ----------
// La cadena de conexión NO está en el repositorio: se guarda con User Secrets
// (dotnet user-secrets set "ConnectionStrings:GimnasioDB" "<cadena>" --project Gimnasio.Api)
var cadenaConexion = builder.Configuration.GetConnectionString("GimnasioDB")
    ?? throw new InvalidOperationException(
        "Falta la cadena de conexión 'GimnasioDB'. Configúrala con dotnet user-secrets (ver README).");

builder.Services.AddDbContext<GimnasioContext>(options =>
    options.UseSqlServer(cadenaConexion));

// ---------- Repositorios (capa Datos) ----------
builder.Services.AddScoped<IMembresiaRepository, MembresiaRepository>();
builder.Services.AddScoped<ISocioRepository, SocioRepository>();

// ---------- Servicios (capa Negocio) ----------
builder.Services.AddScoped<IMembresiaService, MembresiaService>();
builder.Services.AddScoped<ISocioService, SocioService>();

// ---------- AutoMapper (perfil único de la capa Negocio) ----------
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

// ---------- Web API + OpenAPI ----------
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                       // /openapi/v1.json
    app.UseSwaggerUI(options =>             // /swagger
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Gimnasio API v1");
        options.DocumentTitle = "Gimnasio API";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
