using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace DatosNoticias;

public class CnnChile : IExtraccionEGuardadoNoticias
{
    private const string UrlPolitica = $"https://www.cnnchile.com/tag/politica/page/";
    private const string UrlEconomia = $"https://www.cnnchile.com/category/economia/page/";
    private const string UrlEducacion = $"https://www.cnnchile.com/tag/educacion/page/";

    public void ObtenerDatos(int cantidadPaginas, int tipo)
    {
        var listaNoticias = new List<Noticia>();
        var html = new HtmlWeb();
        for (var i = 0; i < cantidadPaginas; i++)
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
                    cargarPagina = html.Load($"{UrlEducacion}{i}/");
                    ExtraerDatos(cargarPagina, html, listaNoticias, "educacion");
                    break;
            }
        }

        GuardarDatos(listaNoticias, listaNoticias.First().Categoria!);
    }

    public void ExtraerDatos(HtmlDocument documentoHtml, HtmlWeb htmlWeb, ICollection<Noticia> listaNoticia,
        string categoria)
    {
        var datos = documentoHtml.DocumentNode.CssSelect("[class='inner-list__item inner-item']")
            .CssSelect("[class='inner-item__title']");
        var urlsNoticias = datos.Select(n => n.CssSelect("a").First().GetAttributeValue("href"));
        foreach (var urls in urlsNoticias)
        {
            try
            {
                var cargaPaginaDeNoticia = htmlWeb.Load(urls);
                var titulo = cargaPaginaDeNoticia.DocumentNode.CssSelect("[class='main-single-header__title']").First()
                    .InnerText.CleanInnerText().ReemplazarCaracteresEspeciales();
                var fecha = cargaPaginaDeNoticia.DocumentNode
                    .CssSelect("[class='main-single-about__item main-single__date']").First().InnerText.Remove(11)
                    .CleanInnerText();
                var contexto = cargaPaginaDeNoticia.DocumentNode.CssSelect("[class='main-single-header__excerpt']")
                    .First()
                    .InnerText.CleanInnerText().ReemplazarCaracteresEspeciales();
                var contenido = cargaPaginaDeNoticia.DocumentNode.CssSelect("[class='main-single-body__content']")
                    .First()
                    .InnerText.CleanInnerText().ReemplazarCaracteresEspeciales();
                var noticia = new Noticia
                    { Titulo = titulo, Fecha = fecha, Contexto = contexto,Contenido = contenido, Categoria = categoria };
                listaNoticia.Add(noticia);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public void GuardarDatos(IEnumerable<Noticia> listaNoticia, string categoria)
    {
        using var escribir =
            new StreamWriter(
                $"C:/Users/hello/RiderProjects/DatosNoticias/DatosNoticias/CNNchile/cnn_chile_{categoria}.csv");
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "   ",
            HasHeaderRecord = true
        };
        using var escribirCsv = new CsvWriter(escribir, config);
        escribirCsv.WriteRecords(listaNoticia);
    }
}