namespace Gimnasio.Entidades;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Correo { get; set; } = null!;

    public string ClaveHash { get; set; } = null!;

    public string Rol { get; set; } = null!;
}