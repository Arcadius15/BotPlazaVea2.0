using BotPlazaVea2._0.Models;
using BotPlazaVea2._0.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotPlazaVea2._0.Clases
{
    public class Executer
    {
        string url = "https://www.plazavea.com.pe/";

        private static List<string> categorias = new List<string>
        {
            "muebles","tecnologia","calzado","deportes","carnes-aves-y-pescados","packs","abarrotes","bebidas","limpieza"
            ,"panaderia-y-pasteleria","frutas-y-verduras","moda","libreria-y-oficina"
        };

        //private static List<string> categorias = new List<string>
        //{
        //    "packs"
        //};

        //private static List<string> categorias = new List<string>
        //{
        //    "muebles","tecnologia","calzado"
        //};


        public async Task obtenerUrls()
        {
            await LoggingService.LogAsync("Cargando Browser...", TipoCodigo.INFO);

            await using var browser = await Browser.getBrowser();

            await LoggingService.LogAsync("Browser Cargado", TipoCodigo.INFO);

            await LoggingService.LogAsync("Log", TipoCodigo.LOG);

            

            await using var page = await browser.NewPageAsync();

            List<Urls> listaUrls = new List<Urls>();

            int pagina = 0;
            int cantidad_productos = 0;
            foreach (var cat in categorias)
            {
                await LoggingService.LogAsync("Abriendo Pagina...", TipoCodigo.INFO);

                await page.GoToAsync(url + cat);

                var waiting = await page.WaitForSelectorAsync(".pagination__item.page-number");

                if (waiting != null)
                {
                    await LoggingService.LogAsync($"Pagina {url + cat} cargada correctamente", TipoCodigo.WARN);
                }

                var paginaRes = await page.EvaluateFunctionAsync("()=>{" +
                    "const a = document.querySelectorAll('.pagination__item.page-number');" +
                    "const res = [];" +
                    "for(let i=0; i<a.length; i++)" +
                    "   res.push(a[i].innerHTML);" +
                    "return res.slice(-1)[0];" +
                    "}");

                pagina = int.Parse(paginaRes.ToString());

                await LoggingService.LogAsync($"{pagina} paginas encontradas", TipoCodigo.WARN);

                var cantidad_total = 50;

                for (int i = 1; i <= pagina; i++)
                {
                    if (cantidad_productos == cantidad_total)
                    {
                        break;
                    }
                    await page.GoToAsync(url + cat + $"?page={i}");
                    await LoggingService.LogAsync("Categoria " + cat + "\n \t" + $"Pagina {i} de {pagina}", TipoCodigo.INFO);
                    await page.WaitForSelectorAsync(".ShowcaseGrid");
                    try
                    {
                        await page.WaitForSelectorAsync(".Showcase__content");
                        var wait = await page.WaitForFunctionAsync("()=>{return " +
                                                "document.querySelectorAll('.ShowcaseGrid')[2].childNodes[1].childNodes.length>0;" +
                                                "return;" +
                                                "}");
                        var estado = wait.JsonValueAsync().Result;
                    }
                    catch (Exception ex)
                    {
                        await LoggingService.LogAsync("Error encontrado: ", TipoCodigo.ERROR, ex);
                    }
                    try
                    {
                        var result = await page.EvaluateFunctionAsync("()=>{" +
                                                    "const b = [];" +
                                                    "document.querySelectorAll('.ShowcaseGrid')[2].childNodes[1].childNodes.forEach(x => {" +
                                                    "   const res = x.childNodes[0].childNodes[1].childNodes[1].childNodes[0].className;" +
                                                    "   if(res.toString()=='Showcase__photo'){ " +
                                                    "       b.push(x.childNodes[0].childNodes[1].childNodes[1].href);" +
                                                    "   }else{" +
                                                    "       b.push(x.childNodes[0].childNodes[1].childNodes[1].childNodes[0].href);" +
                                                    "   };" +
                                                    "});" +
                                                    "return b;}");
                        foreach (var item in result)
                        {
                            if (cantidad_productos == cantidad_total)
                            {
                                break;
                            }
                            if (!String.IsNullOrEmpty(item.ToString().Trim()))
                            {
                                if (!listaUrls.Any(x=>x.url==item.ToString()))
                                {
                                    await LoggingService.LogAsync($"Url {item.Path} de pagina {i} Obtenido", TipoCodigo.HEAD);
                                    await LoggingService.LogAsync(item.ToString(), TipoCodigo.DATA);
                                    Urls urlnew = new Urls() { url = item.ToString(), pagina = i, status = Status.PENDIENTE, endpoint = cat };
                                    listaUrls.Add(urlnew);
                                    cantidad_productos++;
                                }
                                
                            }
                            else
                            {
                                await LoggingService.LogAsync($"Url no encontrado en pagina {i} en la fila {item.Path} de categoria {cat}", TipoCodigo.ERROR);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        await LoggingService.LogAsync($"Error encontrado: ", TipoCodigo.ERROR, ex);
                    }

                }
                cantidad_productos = 0;
            }
            await browser.CloseAsync();
            await LoggingService.LogAsync($"Proceso terminado. Se encontraron {listaUrls.Count} productos.", TipoCodigo.INFO);
            await LoggingService.LogAsync("Iniciando extraccion de data.", TipoCodigo.WARN);
            ImportToDb import = new ImportToDb(listaUrls);
            await import.guardarUrls();
            await buscarUrls();
            await LoggingService.LogAsync($"Proceso terminado.", TipoCodigo.INFO);
        }


        public async Task obtenerProductos(List<string> urls)
        {
            await LoggingService.LogAsync("Cargando Browser...", TipoCodigo.INFO);

            await using var browser = await Browser.getBrowser();

            await LoggingService.LogAsync("Browser Cargado", TipoCodigo.INFO);

            await using var page = await browser.NewPageAsync();

            List<Producto> products = new List<Producto>();

            foreach (var uri in urls)
            {
                try
                {
                    await LoggingService.LogAsync("Abriendo Pagina...", TipoCodigo.INFO);

                    await page.GoToAsync(uri);

                    await page.WaitForSelectorAsync(".bread-crumb");

                    await LoggingService.LogAsync($"Pagina {uri} cargada correctamente", TipoCodigo.WARN);
                    var info = await page.EvaluateFunctionAsync<Producto>("()=>{" +
                    "const categoria = document.querySelectorAll('.bread-crumb')[0].children[0].children[0].children[0].innerText;" +
                    "const subcat = document.querySelectorAll('.bread-crumb')[0].children[0].children[1].children[0].children[0].innerText;" +
                    "const tipo = document.querySelectorAll('.bread-crumb')[0].children[0].children[2].children[0].children[0].innerText;" +
                    "let subti = '';" +
                    "if(document.querySelectorAll('.bread-crumb')[0].children[0].children.length===4)" +
                    "   subti = document.querySelectorAll('.bread-crumb')[0].children[0].children[3].children[0].children[0].innerText;" +
                    "else" +
                    "   subti = 'Promociones';" +
                    "const nompro = document.querySelectorAll('.productName')[0].innerText;" +
                    "const precior = document.querySelectorAll('.ProductCard__content__price')[0].innerText.split(' ')[1];" +
                    "const precioo = document.querySelectorAll('.ProductCard__content__price')[1].innerText.split(' ')[1];" +
                    "const iurl = document.querySelectorAll('#image')[0].children[0].children[0].src;" +
                    "const url = window.location.href;" +
                    "const cod = document.querySelectorAll('.productReference')[0].innerText;" +
                    "let desc = [];" +
                    "if(typeof(document.querySelectorAll('.ProductDetails__specifications')[0].children[0].children[0])!='undefined')" +
                    "   document.querySelectorAll('.ProductDetails__specifications')[0].children[0].children[0].children.forEach(x=>desc.push(x.innerText));" +
                    "let car=[];" +
                    "if(typeof(document.querySelectorAll('.containerHighlight')[0])!='undefined')" +
                    @"  document.querySelectorAll('.containerHighlight')[0].children[0].children[1].children.forEach(x=>car.push(x.innerText));" +
                    "let promo = false;" +
                    "let cond = [];"+
                    "if(typeof(document.querySelectorAll('.ProductCard__promotion')[0])!='undefined'){" +
                    "   promo = true;" +
                    "   if(typeof(document.querySelectorAll('.ProductCard__promotion')[0].children[1])!='undefined')" +
                    "       document.querySelectorAll('.ProductCard__promotion')[0].children[1].children.forEach(x=>cond.push(x.innerText));" +
                    "   else" +
                    "       document.querySelectorAll('.ProductCard__promotion')[0].children[0].children[1].children.forEach(x=>cond.push(x.innerText));" +
                    "};" +
                    "let producto = {" +
                    "   'nombreProducto' : nompro," +
                    "   'precioReg' : precior," +
                    "   'precioOferta' : precioo," +
                    "   'proveedor' : 'Diego Vicente'," +
                    "   'categoria' : categoria," +
                    "   'subcategoria' : subcat," +
                    "   'tipo' : tipo," +
                    "   'subtipo' : subti," +
                    "   'url' : url," +
                    "   'imagenUrl' : iurl," +
                    "   'codigo' : cod," +
                    "   'descripcion' : desc," +
                    "   'caracteristicas' : car," +
                    "   'condiciones': cond," +
                    "   'promocion' : promo" +
                    "};" +
                    "return producto;" +
                    "}");

                    StringBuilder stringBuilder = new StringBuilder();

                    info.descripcion.ForEach(x=>stringBuilder.AppendLine(x));
                    var desc = stringBuilder.ToString();

                    stringBuilder.Clear();

                    info.caracteristicas.ForEach(x=>stringBuilder.AppendLine(x));
                    var car = stringBuilder.ToString();

                    stringBuilder.Clear();

                    var cond = "Sin promociones";

                    if (info.promocion)
                    {
                        info.condiciones.ForEach(x=>stringBuilder.AppendLine(x));
                        cond = stringBuilder.ToString();
                    }

                    await LoggingService.LogAsync(" Producto Obtenido", TipoCodigo.HEAD);
                    await LoggingService.LogAsync("\n" + info.nombreProducto + "\n" +
                        "Precio Oferta : "+info.precioOferta + "\n" +
                        "Precio Regular : " + info.precioReg + "\n" +
                        "Imagen URL : " + info.imagenUrl + "\n" +
                        "Categoria : " + info.categoria + "\n" +
                        "Sub Categoria : " + info.subcategoria + "\n" +
                        "Tipo : " + info.tipo + "\n" +
                        "Sub tipo : " + info.subtipo + "\n" +
                        "Codigo : " + info.codigo + "\n" +
                        "Descripciones : " + desc + "\n" +
                        "Caracteristicas : " + car + "\n" +
                        "Promocion : " + info.promocion + "\n" + 
                        "Condiciones : " + cond,
                        TipoCodigo.DATA);
                    products.Add(info);


                }
                catch (Exception ex)
                {
                    await LoggingService.LogAsync($"Error encontrado en URL: {uri}", TipoCodigo.WARN);
                    await LoggingService.LogAsync($" {ex.Message}", TipoCodigo.ERROR_INFO);
                }
            }

            ImportToDb import = new ImportToDb(products);
            await import.guardarProducto();
            await LoggingService.LogAsync($"Productos Guardados.", TipoCodigo.INFO);
        }

        public async Task buscarUrls()
        {
            foreach (var item in categorias)
            {
                List<string> buscarUrls = new List<string>();
                using (var context = new PlazaVeaContext())
                {
                   var qry =  await context.Urls.Where(x => x.status == Status.PENDIENTE && x.endpoint.Equals(item))
                        .Select(x => x.url)
                        .ToListAsync();
                   buscarUrls = qry;
                }
                await LoggingService.LogAsync($"Obteniendo productos de Categoria {item}.", TipoCodigo.WARN);
                await obtenerProductos(buscarUrls);
                await LoggingService.LogAsync($"Data productos de Categoria {item} obtenida.", TipoCodigo.WARN);
            }
            
        }

    }
}
