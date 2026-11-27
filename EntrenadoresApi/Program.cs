using System.Text;
using EntrenadoresApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Misma clave, emisor y audiencia que Gimnasio.Api (S14). La clave llega por User Secrets o variable de entorno.
var jwt = builder.Configuration.GetSection("Jwt");
var clave = jwt["SecretKey"];
if (string.IsNullOrWhiteSpace(clave) || Encoding.UTF8.GetByteCount(clave) < 32)
    throw new InvalidOperationException("Falta Jwt:SecretKey (minimo 32 bytes). Configurala con User Secrets o la variable Jwt__SecretKey.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            NameClaimType = "sub",
            RoleClaimType = "role",
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

// Todos los endpoints exigen un usuario autenticado, aunque alguien olvide RequireAuthorization.
builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

builder.Services.AddSingleton<EntrenadorRepositorio>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

var entrenadores = app.MapGroup("/api/entrenadores").RequireAuthorization();

// GET /api/entrenadores?especialidad=yoga
entrenadores.MapGet("", (string? especialidad, EntrenadorRepositorio repositorio) =>
    Results.Ok(repositorio.Listar(especialidad)));

// GET /api/entrenadores/3
entrenadores.MapGet("/{id:int}", (int id, EntrenadorRepositorio repositorio) =>
    repositorio.ObtenerPorId(id) is { } entrenador
        ? Results.Ok(entrenador)
        : Results.NotFound(new { mensaje = $"No existe el entrenador {id}." }));

app.Run();
