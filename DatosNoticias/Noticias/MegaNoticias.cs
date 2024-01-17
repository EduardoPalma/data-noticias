using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace DatosNoticias;

public class MegaNoticias : IExtraccionEGuardadoNoticias
{
    private const string UrlPolitica = $"https://www.meganoticias.cl/temas/politica/";
    private const string UrlEconomia = $"https://www.meganoticias.cl/temas/economia/";
    private const string UrlSociedad = $"https://www.meganoticias.cl/temas/educacion/";
    private const string UrlPolicial = $"https://www.meganoticias.cl/temas/policial/";

    public void ObtenerDatos(int cantidadPaginas, int tipo)
    {
        var listaNoticias = new List<Noticia>();
        var html = new HtmlWeb();
        for (var i = 1; i <= cantidadPaginas; i++)
        {
            HtmlDocument cargarPagina;
            var index = i == 1 ? "" : $"?page={i}";
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
        var contenedor = documentoHtml.DocumentNode.CssSelect("[class='box-generica ']");
        var urls = contenedor.Select(c => c.CssSelect("a").First().GetAttributeValue("href"));
        foreach (var url in urls)
        {
            try
            {
                var cargarPagina = htmlWeb.Load(url);
                var titulo = cargarPagina.DocumentNode.CssSelect("[class='contenedor-contenido']").CssSelect("h1")
                    .First().InnerText.CleanInnerText();
                var contexto = "vacio";
                var fecha = cargarPagina.DocumentNode.CssSelect("[class='contenedor-contenido']")
                    .CssSelect("[class='fechaHora']").CssSelect("time").First().GetAttributeValue("datetime")
                    .NormalizarFechaYYDD();
                var contenidoList= cargarPagina.DocumentNode.CssSelect("[class='contenido-nota']").CssSelect("p")
                    .Select(t => t.InnerText.CleanInnerText().Replace("Ir a la siguiente nota",""));
                var contenido = string.Join(" ", contenidoList);
                var noticia = new Noticia
                    { Titulo = titulo, Fecha = fecha,Contexto = contexto,Contenido = contenido, Categoria = categoria };
                listaNoticia.Add(noticia);
            }
            catch (Exception e)
            {
                Console.WriteLine($"error en {url}\n" + e);
            }
        }
    }

    public void GuardarDatos(IEnumerable<Noticia> listaNoticia, string categoria)
    {
        using var escribir =
            new StreamWriter(
                $"C:/Users/hello/RiderProjects/DatosNoticias/DatosNoticias/Meganoticias/mega_noticias_{categoria}.csv");
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "   ",
            HasHeaderRecord = true
        };
        using var escribirCsv = new CsvWriter(escribir, config);
        escribirCsv.WriteRecords(listaNoticia);
    }
}