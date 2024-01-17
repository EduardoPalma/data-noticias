using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace DatosNoticias;

public class Mercurio : IExtraccionEGuardadoNoticias
{
    private const string UrlPolitica = $"https://www.latercera.com/categoria/politica/page/";
    private const string UrlEconomia = $"https://www.latercera.com/etiqueta/economia/page/";
    private const string UrlSociedad = $"https://www.latercera.com/etiqueta/sociedad/page/";

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
                    cargarPagina = html.Load($"{UrlPolitica}{i}/");
                    ExtraerDatos(cargarPagina, html, listaNoticias, "politica");
                    break;
                case 2:
                    cargarPagina = html.Load($"{UrlEconomia}{i}/");
                    ExtraerDatos(cargarPagina, html, listaNoticias, "economia");
                    break;
                default:
                    cargarPagina = html.Load($"{UrlSociedad}{i}/");
                    ExtraerDatos(cargarPagina, html, listaNoticias, "sociedad");
                    break;
            }
        }

        GuardarDatos(listaNoticias, listaNoticias.First().Categoria!);
    }

    public void ExtraerDatos(HtmlDocument documentoHtml, HtmlWeb htmlWeb, ICollection<Noticia> listaNoticia,
        string categoria)
    {
        var datos = documentoHtml.DocumentNode.CssSelect("[class='card | border-bottom float']")
            .CssSelect("[class='headline | width_full hl']");
        var urlsNoticias = datos.Select(n => n.CssSelect("a").First().GetAttributeValue("href"));
        foreach (var urlsNoticia in urlsNoticias)
        {
            try
            {
                var cargarPagina = htmlWeb.Load($"https://www.latercera.com{urlsNoticia}");
                var titulo = cargarPagina.DocumentNode.CssSelect("h1").First().InnerText.CleanInnerText();
                var fecha = cargarPagina.DocumentNode.CssSelect("[class='author d-flex-center m-bot-10 ']")
                    .CssSelect("time").First().GetAttributeValue("datetime")
                    .Replace(" (Coordinated Universal Time)", "").NormalizarFecha();
                var contexto = cargarPagina.DocumentNode.CssSelect("[class='excerpt']").First().InnerText
                    .CleanInnerText();
                var contenidoList = cargarPagina.DocumentNode.CssSelect("[class='paragraph  ']")
                    .Select(x => x.InnerText.CleanInnerText());
                var contenido = string.Join(" ", contenidoList);
                var noticia = new Noticia
                    { Titulo = titulo, Fecha = fecha,Contexto = contexto,Contenido = contenido, Categoria = categoria };
                listaNoticia.Add(noticia);
            }
            catch (Exception)
            {
                Console.WriteLine("error en url https://www.latercera.com" + urlsNoticias);
            }
        }
    }

    public void GuardarDatos(IEnumerable<Noticia> listaNoticia, string categoria)
    {
        using var escribir =
            new StreamWriter(
                $"C:/Users/hello/RiderProjects/DatosNoticias/DatosNoticias/LaTercera/la_tercera_{categoria}.csv");
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "   ",
            HasHeaderRecord = true
        };
        using var escribirCsv = new CsvWriter(escribir, config);
        escribirCsv.WriteRecords(listaNoticia);
    }
}