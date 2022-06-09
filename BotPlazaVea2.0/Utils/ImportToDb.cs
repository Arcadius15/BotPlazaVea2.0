using BotPlazaVea2._0.Clases;
using BotPlazaVea2._0.Models;
using EFCore.BulkExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BotPlazaVea2._0.Utils
{
    public class ImportToDb
    {
        private List<Producto> productos = new List<Producto>();
        private List<Urls> listaUrls =  new List<Urls>();

        public ImportToDb(List<Producto> pro)
        {
            this.productos = pro;
        }
        public ImportToDb(List<Urls> urls)
        {
            this.listaUrls = urls;
        }



        public async Task guardarProducto()
        {
            using (var context = new PlazaVeaContext())
            {
                List<Productos> listaprod = new List<Productos>();
                foreach (var item in productos)
                {
                    Urls? url = await context.Urls.Where(x => x.url.Equals(item.url)).FirstOrDefaultAsync();
                    if (url != null)
                    {
                        if (url.status.Equals(Status.ERROR))
                        {
                            url.status = Status.ERROR;
                            context.Urls.Update(url);
                            await context.SaveChangesAsync();
                            return;
                        }else if (url.status.Equals(Status.ENCONTRADO))
                        {
                            return;
                        }
                    }
                    else{return;}

                    Productos p = new Productos()
                    {
                        nombreProducto = item.nombreProducto,
                        precioOferta = item.precioOferta,
                        precioReg = item.precioReg,
                        categoria = item.categoria,
                        subcategoria = item.subcategoria,
                        tipo = item.tipo,
                        subtipo = item.subtipo,
                        imagenUrl = item.imagenUrl,
                        proveedor = item.proveedor,
                        promocion = item.promocion,
                        idUrl = url.id
                        
                    };
                    
                    p.caracteristicas = new List<Caracteristicas>();
                    p.descripciones = new List<Descripciones>();
                    p.promociones = new List<Promociones>();
                    foreach (var car in item.caracteristicas)
                    {
                        Caracteristicas c = new Caracteristicas();
                        c.caracteristica = car;
                        p.caracteristicas.Add(c);
                    }
                    foreach (var desc in item.descripcion)
                    {
                        Descripciones d = new Descripciones();
                        d.descripcion = desc;
                        p.descripciones.Add(d);
                    }
                    foreach (var prom in item.condiciones)
                    {
                        Promociones pr = new Promociones();
                        pr.condicion = prom;
                        p.promociones.Add(pr);
                    }
                    listaprod.Add(p);
                    url.status = Status.ENCONTRADO;
                    listaUrls.Add(url);

                }

                try
                {
                    if (listaprod.Count==0)
                    {
                        throw new ArgumentNullException("No existen nuevos productos.");
                    }
                    //bulk es para operaciones grandes, pero funciona correctamente para mapear con otros objetos
                    await context.BulkUpdateAsync(listaUrls);
                    await context.AddRangeAsync(listaprod);
                    await context.BulkSaveChangesAsync();
                }
                catch (Exception ex)
                {
                    await LoggingService.LogAsync($"{ex.Message}", TipoCodigo.ERROR);
                }

            }
        }
    
        public async Task guardarUrls()
        {
            using (var context = new PlazaVeaContext())
            {
                List<Urls> lista = new List<Urls>();
                foreach (Urls url in listaUrls)
                {
                    if (!context.Urls.Any(x => x.url == url.url))
                    {
                        lista.Add(url);
                    }
                }
                try
                {
                    if (lista.Count==0)
                    {
                       throw new ArgumentNullException("No existen nuevos Urls.");
                    }
                    await context.BulkInsertAsync(lista);
                }
                catch (Exception ex)
                {

                    await LoggingService.LogAsync($"{ex.Message}", TipoCodigo.ERROR);
                }
                
            }
        }
    
    }
}
