using DatosNoticias.Mappings;
using Elasticsearch.Net;
using Nest;

namespace DatosNoticias.Elastic;

public static class ElasticSearch
{
    private static ElasticClient _client = ConectarElastic("Document-noticias");

    private static ElasticClient ConectarElastic(string indexDefault)
    {
        var pool = new SingleNodeConnectionPool(new Uri($"https://localhost:9200"));
        var settings = new ConnectionSettings(pool)
            .BasicAuthentication("elastic", "GSc_a89P7pd*6*m6Q0oF").EnableApiVersioningHeader()
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .DisableDirectStreaming()
            .DefaultIndex(indexDefault);
        var client = new ElasticClient(settings);
        return client;
    }

    public static void CrearIndice(string indexName)
    {
        var createIndexResponse = _client.Indices.Create(indexName, c => c
            .Map<Document>(m => m
                .AutoMap<Mapping>()
            )
        );
        if (!createIndexResponse.IsValid) Console.WriteLine(createIndexResponse.DebugInformation);
    }

    public static void AgregarDocumento(string indexName, MappingNoticia noticia)
    {
        var indexResponse = _client.Index(noticia, d => d.Index(indexName));
        if (!indexResponse.IsValid) Console.WriteLine(indexResponse.DebugInformation);
    }

    public static void AgregarVariosDocumentos(string indexName, List<Mapping> listaNoticias)
    {
        var bulkIndexResponse = _client.Bulk(b => b
            .Index(indexName)
            .IndexMany(listaNoticias)
        );
        if (!bulkIndexResponse.IsValid) Console.WriteLine(bulkIndexResponse.DebugInformation);
    }

    public static void BusquedaCoincidenciaExacta(string indexName, string query)
    {
        var searchResponsive = _client.Search<MappingNoticia>(p => p
            .Index(indexName)
            .Query(q => q
                .Match(m => m
                    .Field(n => n.Titulo)
                    .Query(query)
                )
            ).Size(10)
        );
        if (!searchResponsive.IsValid) Console.WriteLine(searchResponsive.DebugInformation);
        else
        {
            Console.WriteLine($"Se encontraron {searchResponsive.Total} resultados:");
            foreach (var hit in searchResponsive.Hits)
            {
                Console.WriteLine($"Titulo: {hit.Source.Titulo}");
                Console.WriteLine($"Fecha: {hit.Source.Fecha}");
                Console.WriteLine($"Categoria: {hit.Source.Categoria}");
                Console.WriteLine();
            }
        }
    }

    public static void BusquedaCoincidenciaDifusa(string indexName, string query)
    {
        var searchResponsive = _client.Search<MappingNoticia>(i => i
            .Index(indexName)
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Titulo)
                    .Query(query)
                    .Fuzziness(Fuzziness.Auto)
                )
            )
        );
        if (!searchResponsive.IsValid) Console.WriteLine(searchResponsive.DebugInformation);
        else
        {
            Console.WriteLine($"Se encontraron {searchResponsive.Total} resultados:");

            foreach (var hit in searchResponsive.Hits)
            {
                Console.WriteLine($"Titulo: {hit.Source.Titulo}");
                Console.WriteLine($"Fecha: {hit.Source.Fecha}");
                Console.WriteLine($"Categoria: {hit.Source.Categoria}");
                Console.WriteLine($"Score: {hit.Score}");
                Console.WriteLine();
            }
        }
    }
}