using DatosNoticias.Elastic;
using Nest;

namespace DatosNoticias.Mappings;

public class Mapping : Document
{
    public string? Categoria { get; set; }
    public DateTime Fecha { get; set; }
    public string? Sitio { get; set; }
    public Valoracion Titulo { get; set; } = null!;
    public Valoracion Contexto { get; set; } = null!;
    public Valoracion Contenido { get; set; } = null!;
}