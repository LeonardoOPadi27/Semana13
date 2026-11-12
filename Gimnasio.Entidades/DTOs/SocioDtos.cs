using System.ComponentModel.DataAnnotations;

namespace Gimnasio.Entidades.DTOs;

/// <summary>DTO de lectura: lo que la API devuelve de un socio.</summary>
public class SocioDto
{
    public int IdSocio { get; set; }
    public string Dni { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public int Edad { get; set; }
    public DateOnly FechaInscripcion { get; set; }
    public bool Activo { get; set; }
    public int IdMembresia { get; set; }
    public string Membresia { get; set; } = string.Empty;
}

/// <summary>DTO de entrada para inscribir un socio (siempre nace activo).</summary>
public class SocioCrearDto
{
    [Required(ErrorMessage = "El DNI es obligatorio")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI solo debe contener números")]
    public string Dni { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los nombres son obligatorios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 60 caracteres")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 60 caracteres")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [StringLength(15, ErrorMessage = "El teléfono no puede superar los 15 caracteres")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
    public DateOnly? FechaNacimiento { get; set; }

    [Required(ErrorMessage = "La membresía es obligatoria")]
    [Range(1, int.MaxValue, ErrorMessage = "El IdMembresia debe ser mayor a 0")]
    public int? IdMembresia { get; set; }
}

/// <summary>DTO de entrada para actualizar un socio (el DNI no se modifica).</summary>
public class SocioActualizarDto
{
    [Required(ErrorMessage = "Los nombres son obligatorios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 60 caracteres")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 60 caracteres")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [StringLength(15, ErrorMessage = "El teléfono no puede superar los 15 caracteres")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
    public DateOnly? FechaNacimiento { get; set; }

    [Required(ErrorMessage = "Indica si el socio está activo")]
    public bool? Activo { get; set; }

    [Required(ErrorMessage = "La membresía es obligatoria")]
    [Range(1, int.MaxValue, ErrorMessage = "El IdMembresia debe ser mayor a 0")]
    public int? IdMembresia { get; set; }
}

/// <summary>Filtros opcionales por query string para GET api/socios.</summary>
public class SocioFiltroDto
{
    /// <summary>Busca en nombres o apellidos (contiene).</summary>
    public string? Nombre { get; set; }

    /// <summary>Solo socios de esta membresía.</summary>
    public int? IdMembresia { get; set; }

    /// <summary>true = activos, false = inactivos.</summary>
    public bool? Activo { get; set; }
}
