namespace EntrenadoresApi;

/// <summary>Entrenador del gimnasio. Este microservicio guarda sus datos en memoria, sin base de datos.</summary>
public sealed record Entrenador(
    int IdEntrenador,
    string Nombres,
    string Apellidos,
    string Especialidad,
    string Turno,
    int AniosExperiencia,
    string Correo,
    bool Activo);

/// <summary>Repositorio en memoria (lista fija que se carga al iniciar el servicio).</summary>
public sealed class EntrenadorRepositorio
{
    private readonly List<Entrenador> _entrenadores =
    [
        new(1, "Carlos", "Mendoza Ríos", "Musculación", "Mañana", 8, "carlos.mendoza@gimnasio.test", true),
        new(2, "Lucía", "Paredes Soto", "Funcional", "Tarde", 5, "lucia.paredes@gimnasio.test", true),
        new(3, "Diego", "Huamán Torres", "Spinning", "Noche", 3, "diego.huaman@gimnasio.test", true),
        new(4, "Andrea", "Quispe Vargas", "Yoga", "Mañana", 6, "andrea.quispe@gimnasio.test", true),
        new(5, "Martín", "Salazar León", "Crossfit", "Tarde", 4, "martin.salazar@gimnasio.test", true),
        new(6, "Rosa", "Flores Cárdenas", "Natación", "Noche", 10, "rosa.flores@gimnasio.test", false)
    ];

    public IReadOnlyList<Entrenador> Listar(string? especialidad) =>
        string.IsNullOrWhiteSpace(especialidad)
            ? _entrenadores
            : _entrenadores
                .Where(e => e.Especialidad.Contains(especialidad.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

    public Entrenador? ObtenerPorId(int id) => _entrenadores.FirstOrDefault(e => e.IdEntrenador == id);
}
