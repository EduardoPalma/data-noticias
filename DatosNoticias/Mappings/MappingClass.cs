using CsvHelper.Configuration;

namespace DatosNoticias.Mappings;

public sealed class MappingClass : ClassMap<Mapping>
{
    public MappingClass()
    {
        Map(m => m.Categoria).Name("Categoria");
        Map(m => m.Fecha).Name("Fecha");
        Map(m => m.Sitio).Name("Sitio web");
        References<ValoracionMapTitulo>(m => m.Titulo);
        References<ValoracionMapContexto>(m => m.Contexto);
        References<ValoracionMapContenido>(m => m.Contenido);
    }
}

public sealed class ValoracionMapTitulo : ClassMap<Valoracion>
{
    public ValoracionMapTitulo()
    {
        Map(m => m.Texto).Name("Titulo");
        Map(m => m.ValorSentimiento).Name("Sentimiento titulo");
        Map(m => m.ValorSubjetividad).Name("Subjetividad titulo");
    }
}

public sealed class ValoracionMapContexto : ClassMap<Valoracion>
{
    public ValoracionMapContexto()
    {
        Map(m => m.Texto).Name("Contexto");
        Map(m => m.ValorSentimiento).Name("Sentimiento contexto");
        Map(m => m.ValorSubjetividad).Name("Subjetividad contexto");
    }
}

public sealed class ValoracionMapContenido : ClassMap<Valoracion>
{
    public ValoracionMapContenido()
    {
        Map(m => m.Texto).Name("Contenido");
        Map(m => m.ValorSentimiento).Name("Sentimiento contenido");
        Map(m => m.ValorSubjetividad).Name("Subjetividad contenido");
    }
}