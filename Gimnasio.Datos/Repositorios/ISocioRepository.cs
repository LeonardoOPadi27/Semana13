using Gimnasio.Entidades;

namespace Gimnasio.Datos.Repositorios;

public interface ISocioRepository
{
    Task<List<Socio>> ListarAsync(string? nombre, int? idMembresia, bool? activo);
    Task<List<Socio>> ListarPorMembresiaAsync(int idMembresia);
    Task<Socio?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteDniAsync(string dni);
    Task AgregarAsync(Socio socio);
    Task ActualizarAsync(Socio socio);
    Task EliminarAsync(Socio socio);
}
