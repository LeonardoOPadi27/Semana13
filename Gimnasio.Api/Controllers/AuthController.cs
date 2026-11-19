using System.Security.Claims;
using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gimnasio.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("registro")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TokenDto>> Registrar(RegistroDto dto)
    {
        try
        {
            var token = await authService.RegistrarAsync(dto);
            return CreatedAtAction(nameof(Perfil), token);
        }
        catch (ReglaNegocioException exception)
        {
            return Conflict(new { mensaje = exception.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDto>> IniciarSesion(LoginDto dto)
    {
        var token = await authService.IniciarSesionAsync(dto);
        return token is null
            ? Unauthorized(new { mensaje = "Correo o clave no validos." })
            : Ok(token);
    }

    [HttpGet("perfil")]
    [Authorize]
    [ProducesResponseType(typeof(PerfilDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<PerfilDto> Perfil()
    {
        var correo = User.FindFirst("email")?.Value;
        var rol = User.FindFirst("role")?.Value;
        if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(rol))
            return Forbid();

        return Ok(new PerfilDto { Correo = correo, Rol = rol });
    }
}