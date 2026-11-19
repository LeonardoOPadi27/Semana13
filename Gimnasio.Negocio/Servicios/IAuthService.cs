using Gimnasio.Entidades.DTOs;

namespace Gimnasio.Negocio.Servicios;

public interface IAuthService
{
    Task<TokenDto> RegistrarAsync(RegistroDto dto);

    Task<TokenDto?> IniciarSesionAsync(LoginDto dto);
}