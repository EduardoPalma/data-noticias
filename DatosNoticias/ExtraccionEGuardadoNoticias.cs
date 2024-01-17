using HtmlAgilityPack;

namespace DatosNoticias;

public interface IExtraccionEGuardadoNoticias
{
    void ObtenerDatos(int cantidadPaginas, int tipo);
    void ExtraerDatos(HtmlDocument documentoHtml, HtmlWeb htmlWeb, ICollection<Noticia> listaNoticia, string categoria);
    void GuardarDatos(IEnumerable<Noticia> listaNoticia, string categoria);
}