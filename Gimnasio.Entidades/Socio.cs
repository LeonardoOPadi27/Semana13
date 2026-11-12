using System;
using System.Collections.Generic;

namespace Gimnasio.Entidades;

public partial class Socio
{
    public int IdSocio { get; set; }

    public string Dni { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public DateOnly FechaNacimiento { get; set; }

    public DateOnly FechaInscripcion { get; set; }

    public bool Activo { get; set; }

    public int IdMembresia { get; set; }

    public virtual Membresia IdMembresiaNavigation { get; set; } = null!;
}
