namespace DatosNoticias.Elastic;

public class MappingNoticia : Document
{
    public string Titulo { get; set;}
    public DateTime Fecha { get; set;}
    public string Categoria { get; set; }
    public string Contenido { get; set; }
}