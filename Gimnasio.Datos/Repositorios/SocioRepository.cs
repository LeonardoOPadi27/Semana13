using Gimnasio.Datos.Contexto;
using Gimnasio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class SocioRepository : ISocioRepository
{
    private readonly GimnasioContext _context;

    public SocioRepository(GimnasioContext context)
    {
        _context = context;
    }

    public async Task<List<Socio>> ListarAsync(string? nombre, int? idMembresia, bool? activo)
    {
        IQueryable<Socio> consulta = _context.Socio
            .Include(s => s.IdMembresiaNavigation)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            var texto = nombre.Trim();
            consulta = consulta.Where(s => s.Nombres.Contains(texto) || s.Apellidos.Contains(texto));
        }

        if (idMembresia.HasValue)
            consulta = consulta.Where(s => s.IdMembresia == idMembresia.Value);

        if (activo.HasValue)
            consulta = consulta.Where(s => s.Activo == activo.Value);

        return await consulta
            .OrderBy(s => s.Apellidos)
            .ThenBy(s => s.Nombres)
            .ToListAsync();
    }

    public async Task<List<Socio>> ListarPorMembresiaAsync(int idMembresia)
    {
        return await _context.Socio
            .Include(s => s.IdMembresiaNavigation)
            .AsNoTracking()
            .Where(s => s.IdMembresia == idMembresia)
            .OrderBy(s => s.Apellidos)
            .ToListAsync();
    }

    public async Task<Socio?> ObtenerPorIdAsync(int id)
    {
        return await _context.Socio
            .Include(s => s.IdMembresiaNavigation)
            .FirstOrDefaultAsync(s => s.IdSocio == id);
    }

    public async Task<bool> ExisteDniAsync(string dni)
    {
        return await _context.Socio.AnyAsync(s => s.Dni == dni);
    }

    public async Task AgregarAsync(Socio socio)
    {
        await _context.Socio.AddAsync(socio);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Socio socio)
    {
        // Si viene de ObtenerPorIdAsync ya está rastreada: EF detecta solo los cambios.
        if (_context.Entry(socio).State == EntityState.Detached)
            _context.Socio.Update(socio);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Socio socio)
    {
        _context.Socio.Remove(socio);
        await _context.SaveChangesAsync();
    }
}
