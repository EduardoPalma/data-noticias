using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DatosNoticias.Mappings;

namespace DatosNoticias.Elastic;

public static class IndexarNoticias
{
    public static void Indexar(string name)
    {
        using var leer =
            new StreamReader(
                $@"C:\Users\hello\RiderProjects\DatosNoticias\DatosNoticias\utils\analisis\Analisis_noticias.csv");
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "|",
            HasHeaderRecord = true
        };
        using var leerCsv = new CsvReader(leer, config);
        var options = new TypeConverterOptions { Formats = new[] { "dd.MM.yyyy" } };
        leerCsv.Context.RegisterClassMap<MappingClass>();
        leerCsv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
        var noticias = leerCsv.GetRecords<Mapping>().ToList();
        Console.WriteLine(noticias.Count);
        EnviarDocumentosPorLotes(name,noticias,10);
    }

    public static void MostrarDatos(IList<Mapping> noticias, int cantidad)
    {
        for (var i = 0; i < cantidad; i++)
        {
            Console.WriteLine(noticias[i].Categoria);
            Console.WriteLine(noticias[i].Fecha);
            Console.WriteLine(noticias[i].Sitio);
            Console.WriteLine(noticias[i].Titulo.Texto);
            Console.WriteLine(noticias[i].Titulo.ValorSentimiento);
            Console.WriteLine(noticias[i].Titulo.ValorSubjetividad);
            Console.WriteLine(noticias[i].Contenido.Texto);
            Console.WriteLine(noticias[i].Contenido.ValorSentimiento);
            Console.WriteLine(noticias[i].Contenido.ValorSubjetividad);
            Console.WriteLine("salto");
        }
    }

    public static void EnviarDocumentosPorLotes(string indice, List<Mapping> documentos, int tamanoLote)
    {
        var totalDocumentos = documentos.Count;
        var numeroLotes = (int)Math.Ceiling((double)totalDocumentos / tamanoLote);

        for (var i = 0; i < numeroLotes; i++)
        {
            var indiceInicio = i * tamanoLote;
            var tamanoLoteActual = Math.Min(tamanoLote, totalDocumentos - indiceInicio);
            var loteActual = documentos.GetRange(indiceInicio, tamanoLoteActual);

            ElasticSearch.AgregarVariosDocumentos(indice, loteActual);
        }
    }
}