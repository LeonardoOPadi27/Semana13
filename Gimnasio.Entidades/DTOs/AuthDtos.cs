using System.ComponentModel.DataAnnotations;

namespace Gimnasio.Entidades.DTOs;

public sealed class RegistroDto
{
    [Required, EmailAddress, StringLength(100)]
    public string Correo { get; set; } = string.Empty;

    [Required, StringLength(72, MinimumLength = 10)]
    public string Clave { get; set; } = string.Empty;
}

public sealed class LoginDto
{
    [Required, EmailAddress, StringLength(100)]
    public string Correo { get; set; } = string.Empty;

    [Required, StringLength(72, MinimumLength = 1)]
    public string Clave { get; set; } = string.Empty;
}

public sealed class TokenDto
{
    public string Token { get; set; } = string.Empty;

    public DateTime FechaExpiracion { get; set; }
}

public sealed class PerfilDto
{
    public string Correo { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;
}