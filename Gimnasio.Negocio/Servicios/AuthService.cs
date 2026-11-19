using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gimnasio.Datos.Repositorios;
using Gimnasio.Entidades;
using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Configuracion;
using Gimnasio.Negocio.Excepciones;
using Microsoft.IdentityModel.Tokens;

namespace Gimnasio.Negocio.Servicios;

public sealed class AuthService(IUsuarioRepository usuarios, JwtSettings jwtSettings) : IAuthService
{
    public async Task<TokenDto> RegistrarAsync(RegistroDto dto)
    {
        var correo = NormalizarCorreo(dto.Correo);
        if (await usuarios.BuscarPorCorreoAsync(correo) is not null)
            throw new ReglaNegocioException("Ya existe una cuenta registrada con ese correo.");

        var usuario = new Usuario
        {
            Correo = correo,
            ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Clave),
            Rol = Roles.Socio
        };

        await usuarios.AgregarAsync(usuario);
        await usuarios.GuardarAsync();
        return CrearToken(usuario);
    }

    public async Task<TokenDto?> IniciarSesionAsync(LoginDto dto)
    {
        var usuario = await usuarios.BuscarPorCorreoAsync(NormalizarCorreo(dto.Correo));
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Clave, usuario.ClaveHash))
            return null;

        if (usuario.Rol is not (Roles.Socio or Roles.Recepcionista))
            return null;

        return CrearToken(usuario);
    }

    private TokenDto CrearToken(Usuario usuario)
    {
        var ahora = DateTime.UtcNow;
        var expiracion = ahora.AddMinutes(jwtSettings.ExpirationMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim("role", usuario.Rol)
        };
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            notBefore: ahora,
            expires: expiracion,
            signingCredentials: credenciales);

        return new TokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            FechaExpiracion = expiracion
        };
    }

    private static string NormalizarCorreo(string correo) => correo.Trim().ToLowerInvariant();
}