using AutoMapper;
using Gimnasio.Datos.Repositorios;
using Gimnasio.Entidades;
using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Utilidades;

namespace Gimnasio.Negocio.Servicios;

public class SocioService : ISocioService
{
    public const int EdadMinima = 16;

    private readonly ISocioRepository _socioRepository;
    private readonly IMembresiaRepository _membresiaRepository;
    private readonly IMapper _mapper;

    public SocioService(ISocioRepository socioRepository,
                        IMembresiaRepository membresiaRepository,
                        IMapper mapper)
    {
        _socioRepository = socioRepository;
        _membresiaRepository = membresiaRepository;
        _mapper = mapper;
    }

    public async Task<List<SocioDto>> ListarAsync(SocioFiltroDto filtro)
    {
        var socios = await _socioRepository.ListarAsync(filtro.Nombre, filtro.IdMembresia, filtro.Activo);
        return _mapper.Map<List<SocioDto>>(socios);
    }

    public async Task<SocioDto?> ObtenerPorIdAsync(int id)
    {
        var socio = await _socioRepository.ObtenerPorIdAsync(id);
        return socio is null ? null : _mapper.Map<SocioDto>(socio);
    }

    public async Task<SocioDto> CrearAsync(SocioCrearDto dto)
    {
        // Regla 4: el DNI no se puede repetir
        if (await _socioRepository.ExisteDniAsync(dto.Dni))
            throw new ReglaNegocioException($"Ya existe un socio registrado con el DNI {dto.Dni}.");

        // Regla 5: edad mínima para inscribirse
        ValidarEdad(dto.FechaNacimiento!.Value);

        // Regla 6: solo se inscribe en membresías que existen y están activas
        await ValidarMembresiaDisponibleAsync(dto.IdMembresia!.Value);

        var socio = _mapper.Map<Socio>(dto);
        await _socioRepository.AgregarAsync(socio);

        // Se vuelve a leer para traer el nombre de la membresía (navegación)
        var creado = await _socioRepository.ObtenerPorIdAsync(socio.IdSocio);
        return _mapper.Map<SocioDto>(creado!);
    }

    public async Task<bool> ActualizarAsync(int id, SocioActualizarDto dto)
    {
        var socio = await _socioRepository.ObtenerPorIdAsync(id);
        if (socio is null)
            return false;

        ValidarEdad(dto.FechaNacimiento!.Value);

        // Si cambia de membresía, la nueva debe estar disponible
        if (dto.IdMembresia != socio.IdMembresia)
            await ValidarMembresiaDisponibleAsync(dto.IdMembresia!.Value);

        _mapper.Map(dto, socio);   // actualiza el objeto existente
        await _socioRepository.ActualizarAsync(socio);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var socio = await _socioRepository.ObtenerPorIdAsync(id);
        if (socio is null)
            return false;

        await _socioRepository.EliminarAsync(socio);
        return true;
    }

    private static void ValidarEdad(DateOnly fechaNacimiento)
    {
        if (fechaNacimiento > Fechas.Hoy)
            throw new ReglaNegocioException("La fecha de nacimiento no puede ser futura.");

        if (Fechas.CalcularEdad(fechaNacimiento) < EdadMinima)
            throw new ReglaNegocioException($"El socio debe tener al menos {EdadMinima} años para inscribirse.");
    }

    private async Task ValidarMembresiaDisponibleAsync(int idMembresia)
    {
        var membresia = await _membresiaRepository.ObtenerPorIdAsync(idMembresia);
        if (membresia is null)
            throw new ReglaNegocioException($"La membresía {idMembresia} no existe.");
        if (!membresia.Activo)
            throw new ReglaNegocioException(
                $"La membresía '{membresia.Nombre}' no está activa y no acepta nuevos socios.");
    }
}
