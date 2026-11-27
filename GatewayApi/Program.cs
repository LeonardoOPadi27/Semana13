using System.Text;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Rutas del gateway. En la nube se usa ocelot.Production.json, que apunta a los hosts publicos.
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// El gateway valida el JWT antes de reenviar (misma clave, emisor y audiencia que Gimnasio.Api).
var jwt = builder.Configuration.GetSection("Jwt");
var clave = jwt["SecretKey"];
if (string.IsNullOrWhiteSpace(clave) || Encoding.UTF8.GetByteCount(clave) < 32)
    throw new InvalidOperationException("Falta Jwt:SecretKey (minimo 32 bytes). Configurala con User Secrets o la variable Jwt__SecretKey.");

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
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
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();
await app.UseOcelot();
app.Run();
