using System.Text;
using DatosNoticias;
using DatosNoticias.Elastic;
using Elasticsearch.Net;
using Nest;

/*
//var theClinic = new TheClinic();
//theClinic.ObtenerDatos(40,2);
//theClinic.ObtenerDatos(177,3);

var cnnChile = new CnnChile();
cnnChile.ObtenerDatos(305,1);
cnnChile.ObtenerDatos(550,2);
cnnChile.ObtenerDatos(142,3);


/*var mercurio = new Mercurio();

mercurio.ObtenerDatos(600,1);
mercurio.ObtenerDatos(235,2);
mercurio.ObtenerDatos(144,3);

var megaNoticias = new MegaNoticias();
megaNoticias.ObtenerDatos(57,1);
megaNoticias.ObtenerDatos(82,2);
megaNoticias.ObtenerDatos(34, 3);
megaNoticias.ObtenerDatos(428,4);
*/
/*var andradio = new AdnRadio();
andradio.ObtenerDatos(1000,1);
andradio.ObtenerDatos(450,2);
andradio.ObtenerDatos(65,3);
andradio.ObtenerDatos(217,4);*/

//ElasticSearch.CrearIndice("noticias_sen_sub");
IndexarNoticias.Indexar("index_sentiment");
//ElasticSearch.BusquedaCoincidenciaExacta("pruebas-varios","crecimiento");
//ElasticSearch.BusquedaCoincidenciaDifusa("pruebas-varios","actividad educativa y el congreso");