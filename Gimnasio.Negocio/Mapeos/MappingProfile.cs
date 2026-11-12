using AutoMapper;
using Gimnasio.Entidades;
using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Utilidades;

namespace Gimnasio.Negocio.Mapeos;

/// <summary>
/// Único perfil de AutoMapper de la solución: todos los mapeos entre entidades y DTOs.
/// No se usa ReverseMap(): los DTOs de lectura nunca regresan a la base de datos
/// y los DTOs de entrada nunca se devuelven al cliente.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ---------- Membresía ----------

        // 1) Entidad -> DTO de lectura (lo que la API devuelve)
        CreateMap<Membresia, MembresiaDto>()
            .ForMember(dest => dest.CantidadSocios,
                       opt => opt.MapFrom(src => src.Socio.Count))
            .ForMember(dest => dest.CostoTotal,
                       opt => opt.MapFrom(src => src.PrecioMensual * src.DuracionMeses));

        // 2) DTO de entrada (crear) -> Entidad
        CreateMap<MembresiaCrearDto, Membresia>()
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre.Trim()))
            .ForMember(dest => dest.IdMembresia, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.Socio, opt => opt.Ignore());

        // 3) DTO de entrada (actualizar) -> Entidad existente
        CreateMap<MembresiaActualizarDto, Membresia>()
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre.Trim()))
            .ForMember(dest => dest.IdMembresia, opt => opt.Ignore())
            .ForMember(dest => dest.Socio, opt => opt.Ignore());

        // ---------- Socio ----------

        // 4) Entidad -> DTO de lectura
        CreateMap<Socio, SocioDto>()
            .ForMember(dest => dest.NombreCompleto,
                       opt => opt.MapFrom(src => src.Nombres + " " + src.Apellidos))
            .ForMember(dest => dest.Membresia,
                       opt => opt.MapFrom(src => src.IdMembresiaNavigation.Nombre))
            .ForMember(dest => dest.Edad,
                       opt => opt.MapFrom(src => Fechas.CalcularEdad(src.FechaNacimiento)));

        // 5) DTO de entrada (crear) -> Entidad
        CreateMap<SocioCrearDto, Socio>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLower()))
            .ForMember(dest => dest.IdSocio, opt => opt.Ignore())
            .ForMember(dest => dest.FechaInscripcion, opt => opt.MapFrom(_ => Fechas.Hoy))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.IdMembresiaNavigation, opt => opt.Ignore());

        // 6) DTO de entrada (actualizar) -> Entidad existente
        CreateMap<SocioActualizarDto, Socio>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLower()))
            .ForMember(dest => dest.IdSocio, opt => opt.Ignore())
            .ForMember(dest => dest.Dni, opt => opt.Ignore())
            .ForMember(dest => dest.FechaInscripcion, opt => opt.Ignore())
            .ForMember(dest => dest.IdMembresiaNavigation, opt => opt.Ignore());
    }
}
