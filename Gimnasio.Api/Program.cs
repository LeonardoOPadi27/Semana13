using System.Text;
using Gimnasio.Api;
using Gimnasio.Datos.Contexto;
using Gimnasio.Datos.Repositorios;
using Gimnasio.Negocio.Configuracion;
using Gimnasio.Negocio.Mapeos;
using Gimnasio.Negocio.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var cadenaConexion = builder.Configuration.GetConnectionString("GimnasioDB")
    ?? throw new InvalidOperationException(
        "Falta la cadena de conexion 'GimnasioDB'. Configurala con User Secrets (ver README)." );

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Falta la configuracion Jwt.");
if (Encoding.UTF8.GetByteCount(jwtSettings.SecretKey) < 32)
    throw new InvalidOperationException("Jwt:SecretKey debe tener al menos 32 bytes y permanecer fuera del repositorio.");
if (string.IsNullOrWhiteSpace(jwtSettings.Issuer) || string.IsNullOrWhiteSpace(jwtSettings.Audience) || jwtSettings.ExpirationMinutes <= 0)
    throw new InvalidOperationException("La configuracion Jwt requiere emisor, audiencia y minutos de vida positivos.");

builder.Services.AddDbContext<GimnasioContext>(options => options.UseSqlServer(cadenaConexion));
builder.Services.AddScoped<IMembresiaRepository, MembresiaRepository>();
builder.Services.AddScoped<ISocioRepository, SocioRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IMembresiaService, MembresiaService>();
builder.Services.AddScoped<ISocioService, SocioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            NameClaimType = "sub",
            RoleClaimType = "role",
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        var requiereAutenticacion = metadata.OfType<IAuthorizeData>().Any();
        var esPublico = metadata.OfType<IAllowAnonymous>().Any();
        if (requiereAutenticacion && !esPublico)
        {
            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
            });
        }

        return Task.CompletedTask;
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Gimnasio API v1");
        options.DocumentTitle = "Gimnasio API - Seguridad JWT";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();