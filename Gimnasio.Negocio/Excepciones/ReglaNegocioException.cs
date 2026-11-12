namespace Gimnasio.Negocio.Excepciones;

/// <summary>
/// Se lanza cuando una operación incumple una regla de negocio del gimnasio.
/// El controlador la convierte en 409 Conflict.
/// </summary>
public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje)
    {
    }
}
