using System.Text;
using System.Text.RegularExpressions;

namespace DatosNoticias;

public static partial class StringExtensiones
{
    public static string Autf(string texto)
    {
        return Encoding.GetEncoding("iso-8859-1")
            .GetString(Encoding.Default.GetBytes(MyRegex().Replace(texto, m => ((int)m.Value[0]).ToString("x"))));
    }

    [GeneratedRegex("[^ -~]")]
    private static partial Regex MyRegex();

    public static string ReemplazarCaracteresEspeciales(this string texto)
    {
        texto = Regex.Replace(texto, "Ã¡", "á");
        texto = Regex.Replace(texto, "Ã©", "é");
        texto = Regex.Replace(texto, "Ã­", "í");
        texto = Regex.Replace(texto, "Ã³", "ó");
        texto = Regex.Replace(texto, "Ãº", "ú");
        texto = Regex.Replace(texto, "Ã±", "ñ");
        texto = Regex.Replace(texto, "Â", "");
        return texto;
    }

    public static string ReemplazarCaracteresEspecialesUTF(this string texto)
    {
        var isoEncoding = Encoding.GetEncoding("ISO-8859-1");
        var utf8Encoding = Encoding.UTF8;

        var isoBytes = isoEncoding.GetBytes(texto);
        var utf8Bytes = Encoding.Convert(isoEncoding, utf8Encoding, isoBytes);

        return utf8Encoding.GetString(utf8Bytes);
    }
}