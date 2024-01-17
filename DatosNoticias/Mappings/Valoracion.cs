using DatosNoticias.Elastic;

namespace DatosNoticias.Mappings;

public class Valoracion
{
    public string? Texto { get; set; }
    public double ValorSentimiento { get; set; }
    public double ValorSubjetividad { get; set; }
}