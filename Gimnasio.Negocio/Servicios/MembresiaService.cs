using AutoMapper;
using Gimnasio.Datos.Repositorios;
using Gimnasio.Entidades;
using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Excepciones;

namespace Gimnasio.Negocio.Servicios;

public class MembresiaService : IMembresiaService
{
    private readonly IMembresiaRepository _membresiaRepository;
    private readonly ISocioRepository _socioRepository;
    private readonly IMapper _mapper;

    public MembresiaService(IMembresiaRepository membresiaRepository,
                            ISocioRepository socioRepository,
                            IMapper mapper)
    {
        _membresiaRepository = membresiaRepository;
        _socioRepository = socioRepository;
        _mapper = mapper;
    }

    public async Task<List<MembresiaDto>> ListarAsync()
    {
        var membresias = await _membresiaRepository.ListarAsync();
        return _mapper.Map<List<MembresiaDto>>(membresias);
    }

    public async Task<MembresiaDto?> ObtenerPorIdAsync(int id)
    {
        var membresia = await _membresiaRepository.ObtenerPorIdAsync(id);
        return membresia is null ? null : _mapper.Map<MembresiaDto>(membresia);
    }

    public async Task<List<SocioDto>?> ListarSociosAsync(int idMembresia)
    {
        if (!await _membresiaRepository.ExisteAsync(idMembresia))
            return null;

        var socios = await _socioRepository.ListarPorMembresiaAsync(idMembresia);
        return _mapper.Map<List<SocioDto>>(socios);
    }

    public async Task<MembresiaDto> CrearAsync(MembresiaCrearDto dto)
    {
        // Regla 1: no pueden existir dos membresías con el mismo nombre
        if (await _membresiaRepository.ExisteNombreAsync(dto.Nombre))
            throw new ReglaNegocioException($"Ya existe una membresía llamada '{dto.Nombre.Trim()}'.");

        var membresia = _mapper.Map<Membresia>(dto);
        await _membresiaRepository.AgregarAsync(membresia);

        return _mapper.Map<MembresiaDto>(membresia);
    }

    public async Task<bool> ActualizarAsync(int id, MembresiaActualizarDto dto)
    {
        var membresia = await _membresiaRepository.ObtenerPorIdAsync(id);
        if (membresia is null)
            return false;

        // Regla 1: el nuevo nombre no puede repetirse con otra membresía
        if (await _membresiaRepository.ExisteNombreAsync(dto.Nombre, excluirId: id))
            throw new ReglaNegocioException($"Ya existe otra membresía llamada '{dto.Nombre.Trim()}'.");

        // Regla 2: no se puede desactivar una membresía que todavía tiene socios activos
        if (dto.Activo == false && membresia.Socio.Any(s => s.Activo))
            throw new ReglaNegocioException(
                "No se puede desactivar la membresía porque tiene socios activos. Cámbialos de membresía primero.");

        _mapper.Map(dto, membresia);   // actualiza el objeto existente
        await _membresiaRepository.ActualizarAsync(membresia);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var membresia = await _membresiaRepository.ObtenerPorIdAsync(id);
        if (membresia is null)
            return false;

        // Regla 3: no se elimina una membresía que tiene socios inscritos
        if (await _membresiaRepository.TieneSociosAsync(id))
            throw new ReglaNegocioException(
                $"No se puede eliminar la membresía '{membresia.Nombre}' porque tiene socios inscritos.");

        await _membresiaRepository.EliminarAsync(membresia);
        return true;
    }
}
