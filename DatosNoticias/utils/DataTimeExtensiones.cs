using System.Globalization;

namespace DatosNoticias;

public static class DataTimeExtensiones
{
    public static string NormalizarFecha(this string fecha)
    {
        var dateTime = DateTime.ParseExact(fecha, "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz", CultureInfo.InvariantCulture);
        return dateTime.ToString("dd.MM.yyyy");
    }

    public static string NormalizarFechaYYDD(this string fecha)
    {
        var dateTime = DateTime.ParseExact(fecha, "yyyy-MM-dd'T'HH:mm:sszzz HH:mm", CultureInfo.InvariantCulture);
        return dateTime.ToString("dd.MM.yyyy");
    }

    public static string NormalizarFechaADN(this string fecha)
    {
        var cultura = new CultureInfo("es-ES");
        var dateTime = DateTime.ParseExact(fecha, "dddd d 'de' MMM, yyyy - HH:mm", cultura);
        return dateTime.ToString("dd.MM.yyyy");
    }
}