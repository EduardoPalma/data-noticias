using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace DatosNoticias;

public class TheClinic : IExtraccionEGuardadoNoticias
{
    private const string UrlPolitica = $"https://www.theclinic.cl/noticias/politica/";
    private const string UrlEconomia = $"https://www.theclinic.cl/etiqueta/economia/";
    private const string UrlEducacion = $"https://www.theclinic.cl/etiqueta/educacion/";

    public void ObtenerDatos(int cantidadPaginas, int tipo)
    {
        var listaNoticias = new List<Noticia>();
        var html = new HtmlWeb();
        for (var i = 1; i <= cantidadPaginas; i++)
        {
            HtmlDocument cargarPagina;
            switch (tipo)
            {
                case 1:
                    cargarPagina =
                        html.Load(i == 1 ? UrlPolitica : $"https://www.theclinic.cl/noticias/politica/page/{i}/");
                    ExtraerDatos(cargarPagina!, html, listaNoticias, "Politica");
                    break;
                case 2:
                    cargarPagina =
                        html.Load(i == 1 ? UrlEconomia : $"https://www.theclinic.cl/etiqueta/economia/page/{i}/");
                    ExtraerDatos(cargarPagina!, html, listaNoticias, "Economia");
                    break;
                default:
                    cargarPagina =
                        html.Load(i == 1 ? UrlEducacion : $"https://www.theclinic.cl/etiqueta/educacion/page/{i}/");
                    ExtraerDatos(cargarPagina!, html, listaNoticias, "Educacion");
                    break;
            }
        }

        GuardarDatos(listaNoticias, listaNoticias.First().Categoria!);
    }

    public void ExtraerDatos(HtmlDocument documentoHtml, HtmlWeb htmlWeb, ICollection<Noticia> listaNoticia,
        string categoria)
    {
        var datos = documentoHtml.DocumentNode.CssSelect("[class='listado']").CssSelect("article");
        var urlDeDatoNoticia = datos.Select(n => n.CssSelect("a").First().GetAttributeValue("href"));
        foreach (var urls in urlDeDatoNoticia)
        {
            Console.WriteLine(urls);
            var cargarPaginaNoticia = htmlWeb.Load(urls);
            var titulo = cargarPaginaNoticia.DocumentNode.CssSelect("h1").First().InnerText.Replace("&#8220;", "")
                .Replace("&#8221", "");
            var fecha = cargarPaginaNoticia.DocumentNode.CssSelect("[class='fecha']").First().InnerText;
            try
            {
                var contexto = cargarPaginaNoticia.DocumentNode.CssSelect("[class='bajada']").First().InnerText
                    .CleanInnerText();
                var contenido = cargarPaginaNoticia.DocumentNode.CssSelect("[class='the-content']").First().InnerText
                    .CleanInnerText();
                var noticia = new Noticia
                    { Titulo = titulo, Fecha = fecha, Contenido = $"{contexto} {contenido}", Categoria = categoria };
                listaNoticia.Add(noticia);
            }
            catch (Exception)
            {
                var contenido = cargarPaginaNoticia.DocumentNode.CssSelect("[class='the-content']").First().InnerText
                    .CleanInnerText();
                var noticia = new Noticia
                    { Titulo = titulo, Fecha = fecha, Contenido = contenido, Categoria = categoria };
                listaNoticia.Add(noticia);
            }
        }
    }

    public void GuardarDatos(IEnumerable<Noticia> listaNoticia, string categoria)
    {
        using var escribir =
            new StreamWriter(
                $"C:/Users/hello/RiderProjects/DatosNoticias/DatosNoticias/NoticiasTheClinic/the_clinic_{categoria}.csv");
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "   ",
            HasHeaderRecord = true
        };
        using var escribirCsv = new CsvWriter(escribir, config);
        escribirCsv.WriteRecords(listaNoticia);
    }
}