using Gimnasio.Datos.Contexto;
using Gimnasio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public sealed class UsuarioRepository(GimnasioContext context) : IUsuarioRepository
{
    public Task<Usuario?> BuscarPorCorreoAsync(string correo) =>
        context.Usuario.AsNoTracking().SingleOrDefaultAsync(usuario => usuario.Correo == correo);

    public async Task AgregarAsync(Usuario usuario) => await context.Usuario.AddAsync(usuario);

    public Task GuardarAsync() => context.SaveChangesAsync();
}