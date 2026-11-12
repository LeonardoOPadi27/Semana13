using Gimnasio.Datos.Contexto;
using Gimnasio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class MembresiaRepository : IMembresiaRepository
{
    private readonly GimnasioContext _context;

    public MembresiaRepository(GimnasioContext context)
    {
        _context = context;
    }

    public async Task<List<Membresia>> ListarAsync()
    {
        return await _context.Membresia
            .Include(m => m.Socio)
            .AsNoTracking()
            .OrderBy(m => m.PrecioMensual)
            .ToListAsync();
    }

    public async Task<Membresia?> ObtenerPorIdAsync(int id)
    {
        return await _context.Membresia
            .Include(m => m.Socio)
            .FirstOrDefaultAsync(m => m.IdMembresia == id);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Membresia.AnyAsync(m => m.IdMembresia == id);
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null)
    {
        var nombreNormalizado = nombre.Trim().ToLower();
        return await _context.Membresia
            .Where(m => excluirId == null || m.IdMembresia != excluirId)
            .AnyAsync(m => m.Nombre.ToLower() == nombreNormalizado);
    }

    public async Task<bool> TieneSociosAsync(int id)
    {
        return await _context.Socio.AnyAsync(s => s.IdMembresia == id);
    }

    public async Task AgregarAsync(Membresia membresia)
    {
        await _context.Membresia.AddAsync(membresia);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Membresia membresia)
    {
        // Si viene de ObtenerPorIdAsync ya está rastreada: EF detecta solo los cambios.
        if (_context.Entry(membresia).State == EntityState.Detached)
            _context.Membresia.Update(membresia);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Membresia membresia)
    {
        _context.Membresia.Remove(membresia);
        await _context.SaveChangesAsync();
    }
}
