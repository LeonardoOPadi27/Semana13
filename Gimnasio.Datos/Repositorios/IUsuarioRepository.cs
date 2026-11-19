using Gimnasio.Entidades;

namespace Gimnasio.Datos.Repositorios;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorCorreoAsync(string correo);

    Task AgregarAsync(Usuario usuario);

    Task GuardarAsync();
}