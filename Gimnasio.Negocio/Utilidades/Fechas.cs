namespace Gimnasio.Negocio.Utilidades;

public static class Fechas
{
    public static DateOnly Hoy => DateOnly.FromDateTime(DateTime.Today);

    /// <summary>Edad en años cumplidos a la fecha de hoy.</summary>
    public static int CalcularEdad(DateOnly fechaNacimiento)
    {
        var hoy = Hoy;
        var edad = hoy.Year - fechaNacimiento.Year;
        if (fechaNacimiento > hoy.AddYears(-edad))
            edad--;
        return edad;
    }
}
