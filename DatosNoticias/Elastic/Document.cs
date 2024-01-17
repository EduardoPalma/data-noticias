using Nest;

namespace DatosNoticias.Elastic;

public abstract class Document
{
    public JoinField Join { get; set; }
}