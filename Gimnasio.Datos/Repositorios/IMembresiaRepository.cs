using Gimnasio.Entidades;

namespace Gimnasio.Datos.Repositorios;

public interface IMembresiaRepository
{
    Task<List<Membresia>> ListarAsync();
    Task<Membresia?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null);
    Task<bool> TieneSociosAsync(int id);
    Task AgregarAsync(Membresia membresia);
    Task ActualizarAsync(Membresia membresia);
    Task EliminarAsync(Membresia membresia);
}
