using Gimnasio.Entidades.DTOs;

namespace Gimnasio.Negocio.Servicios;

public interface ISocioService
{
    Task<List<SocioDto>> ListarAsync(SocioFiltroDto filtro);
    Task<SocioDto?> ObtenerPorIdAsync(int id);
    Task<SocioDto> CrearAsync(SocioCrearDto dto);
    /// <summary>false si el socio no existe.</summary>
    Task<bool> ActualizarAsync(int id, SocioActualizarDto dto);
    /// <summary>false si el socio no existe.</summary>
    Task<bool> EliminarAsync(int id);
}
