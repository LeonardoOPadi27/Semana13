using Gimnasio.Entidades.DTOs;

namespace Gimnasio.Negocio.Servicios;

public interface IMembresiaService
{
    Task<List<MembresiaDto>> ListarAsync();
    Task<MembresiaDto?> ObtenerPorIdAsync(int id);
    /// <summary>Socios de una membresía; null si la membresía no existe.</summary>
    Task<List<SocioDto>?> ListarSociosAsync(int idMembresia);
    Task<MembresiaDto> CrearAsync(MembresiaCrearDto dto);
    /// <summary>false si la membresía no existe.</summary>
    Task<bool> ActualizarAsync(int id, MembresiaActualizarDto dto);
    /// <summary>false si la membresía no existe.</summary>
    Task<bool> EliminarAsync(int id);
}
