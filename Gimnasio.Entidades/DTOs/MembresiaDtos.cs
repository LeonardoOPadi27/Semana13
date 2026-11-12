using System.ComponentModel.DataAnnotations;

namespace Gimnasio.Entidades.DTOs;

/// <summary>DTO de lectura: lo que la API devuelve de una membresía.</summary>
public class MembresiaDto
{
    public int IdMembresia { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal PrecioMensual { get; set; }
    public int DuracionMeses { get; set; }
    public decimal CostoTotal { get; set; }
    public bool Activo { get; set; }
    public int CantidadSocios { get; set; }
}

/// <summary>DTO de entrada para crear una membresía (siempre nace activa).</summary>
public class MembresiaCrearDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El precio mensual es obligatorio")]
    [Range(10, 1000, ErrorMessage = "El precio mensual debe estar entre 10 y 1000")]
    public decimal? PrecioMensual { get; set; }

    [Required(ErrorMessage = "La duración es obligatoria")]
    [Range(1, 24, ErrorMessage = "La duración debe estar entre 1 y 24 meses")]
    public int? DuracionMeses { get; set; }
}

/// <summary>DTO de entrada para actualizar una membresía existente.</summary>
public class MembresiaActualizarDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El precio mensual es obligatorio")]
    [Range(10, 1000, ErrorMessage = "El precio mensual debe estar entre 10 y 1000")]
    public decimal? PrecioMensual { get; set; }

    [Required(ErrorMessage = "La duración es obligatoria")]
    [Range(1, 24, ErrorMessage = "La duración debe estar entre 1 y 24 meses")]
    public int? DuracionMeses { get; set; }

    [Required(ErrorMessage = "Indica si la membresía está activa")]
    public bool? Activo { get; set; }
}
