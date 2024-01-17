using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace DatosNoticias;

public class AdnRadio : IExtraccionEGuardadoNoticias
{
    private const string UrlPolitica = $"https://www.adnradio.cl/category/politica/";
    private const string UrlEconomia = $"https://www.adnradio.cl/category/economia/";
    private const string UrlSociedad = $"https://www.adnradio.cl/category/cultura-y-educacion/";
    private const string UrlPolicial = $"https://www.adnradio.cl/category/policial/";

    public void ObtenerDatos(int cantidadPaginas, int tipo)
    {
        var listaNoticias = new List<Noticia>();
        var html = new HtmlWeb();
        for (var i = 1; i <= cantidadPaginas; i++)
        {
            HtmlDocument cargarPagina;
            var index = i == 1 ? "" : $"page/{i}/";
            switch (tipo)
            {
                case 1:
                    cargarPagina = html.Load($"{UrlPolitica}{index}");
                    ExtraerDatos(cargarPagina, html, listaNoticias, "politica");
                    break;
                case 2:
                    cargarPagina = html.Load($"{UrlEconomia}{index}");
                    ExtraerDatos(cargarPagina, html, listaNoticias, "economia");
                    break;
                case 3:
                    cargarPagina = html.Load($"{UrlSociedad}{index}");
                    ExtraerDatos(cargarPagina, html, listaNoticias, "educacion");
                    break;
                default:
                    cargarPagina = html.Load($"{UrlPolicial}{index}");
                    ExtraerDatos(cargarPagina, html, listaNoticias, "policial");
                    break;
            }
        }
        GuardarDatos(listaNoticias, listaNoticias.First().Categoria!);
    }

    public void ExtraerDatos(HtmlDocument documentoHtml, HtmlWeb htmlWeb, ICollection<Noticia> listaNoticia,
        string categoria)
    {
        var contenedorLeft = documentoHtml.DocumentNode.CssSelect("[class='main-section-alt__left']");
        var contenedorRight = documentoHtml.DocumentNode.CssSelect("[class='main-section-alt u-space-bottom-30']");
        var listaUrls = new List<string>();
        ListaUrls(listaUrls, contenedorLeft, true);
        ListaUrls(listaUrls, contenedorRight, false);
        foreach (var url in listaUrls)
        {
            try
            {
                var cargarPagina = htmlWeb.Load(url);
                var titulo = cargarPagina.DocumentNode.CssSelect("[class='the-single__title']").First().InnerText
                    .CleanInnerText().ReemplazarCaracteresEspeciales();
                var contexto = cargarPagina.DocumentNode.CssSelect("[class='the-single__excerpt']").CssSelect("p")
                    .First()
                    .InnerText.CleanInnerText().ReemplazarCaracteresEspeciales();
                var regexFecha = new Regex(@"\b\p{L}+\s\d{1,2}\sde\s\p{L}+,\s\d{4}\s-\s\d{1,2}:\d{2}\b");
                var nodos = cargarPagina.DocumentNode.CssSelect("[class='the-single-info__item']")
                    .Select(x => x.InnerText.CleanInnerText());
                var fecha = FechaReal(nodos, regexFecha).NormalizarFechaADN();
                var contenido = string.Join(" ",cargarPagina.DocumentNode.CssSelect("[class='the-single-content__body']")
                    .CssSelect("p").Select(c => c.InnerText.CleanInnerText().ReemplazarCaracteresEspeciales()));
                var noticia = new Noticia
                    { Titulo = titulo, Fecha = fecha,Contexto = contexto,Contenido = contenido, Categoria = categoria };
                listaNoticia.Add(noticia);
            }
            catch (Exception e)
            {
                Console.WriteLine($"error en {url}\n {e}");
                
            }
        }
    }

    private string FechaReal(IEnumerable<string> nodos, Regex regexFecha)
    {
        foreach (var f in nodos)
        {
            var match = regexFecha.Match(f);
            if (match.Success) return match.Value.Replace("rcoles", "Miércoles").Replace("bado", "Sábado");
        }

        return "";
    }

    private void ListaUrls(List<string> listaUrls, IEnumerable<HtmlNode> contenedor, bool existe)
    {
        foreach (var contener in contenedor)
        {
            if (existe)
            {
                var url = contener.CssSelect("[class='alt-card main-section-alt__item']").CssSelect("a").First()
                    .GetAttributeValue("href");
                var urls = contener.CssSelect("[class='lateral-card  main-section-alt__item']")
                    .Select(u => u.CssSelect("a").First().GetAttributeValue("href"));
                listaUrls.Add(url);
                listaUrls.AddRange(urls);
            }
            else
            {
                var urls = contener.CssSelect("[class='lateral-card  main-section-alt__item']")
                    .Select(u => u.CssSelect("a").First().GetAttributeValue("href"));
                listaUrls.AddRange(urls);
            }
        }
    }

    public void GuardarDatos(IEnumerable<Noticia> listaNoticia, string categoria)
    {
        using var escribir =
            new StreamWriter(
                $"C:/Users/hello/RiderProjects/DatosNoticias/DatosNoticias/adnradio/andradio_{categoria}.csv");
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "   ",
            HasHeaderRecord = true
        };
        using var escribirCsv = new CsvWriter(escribir, config);
        escribirCsv.WriteRecords(listaNoticia);
    }
}