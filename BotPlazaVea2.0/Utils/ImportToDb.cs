using BotPlazaVea2._0.Clases;
using BotPlazaVea2._0.Models;
using EFCore.BulkExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotPlazaVea2._0.Utils
{
    public class ImportToDb
    {
        public async Task guardarProducto(List<Producto> productos)
        {
            using (var context = new PlazaVeaContext())
            {
                List<Urls> listaurls = new List<Urls>();
                List<Productos> listaprod = new List<Productos>();
                foreach (var item in productos)
                {
                    Urls url = new Urls();
                    url.url = item.url;
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
                        promocion = item.promocion
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
                    url.Producto = p;
                    if (!context.Urls.Any(x=>x.url == url.url))
                    {
                        listaurls.Add(url);
                    }
                    
                }

                try
                {
                    if (listaurls.Count==0)
                    {
                        throw new ArgumentNullException("No existen nuevas urls.");
                    }
                    //bulk es para operaciones grandes, pero funciona correctamente para mapear con otros objetos
                    await context.AddRangeAsync(listaurls);
                    await context.BulkSaveChangesAsync();
                    //await context.BulkSaveChangesAsync(new BulkConfig { OmitClauseExistsExcept = true});
                    //bulk extensions es de paga
                    /*await context.BulkMergeAsync(listaurls, options =>
                    {
                        options.InsertIfNotExists = true;
                        options.ErrorMode = Z.BulkOperations.ErrorModeType.RetrySingleAndContinue;
                        options.IncludeGraph = true;
                        options.IncludeGraphOperationBuilder = operation =>
                        {
                            switch (operation)
                            {
                                case BulkOperation<Urls> url:
                                    url.InsertIfNotExists = true;
                                    url.ColumnPrimaryKeyExpression = x => new { x.url };
                                    url.AutoMapOutputDirection = true;
                                    break;
                                case BulkOperation<Productos> p:
                                    p.InsertIfNotExists = true;
                                    p.ColumnPrimaryKeyExpression = x =>
                                        new {
                                            x.imagenUrl,
                                            x.nombreProducto,
                                            x.proveedor,
                                            x.subcategoria,
                                            x.categoria,
                                            x.tipo,
                                            x.subtipo,
                                            x.precioOferta,
                                            x.precioReg
                                        };
                                    p.AutoMapOutputDirection = true;
                                    break;

                            }
                        };
                    });
                    */
                }
                catch (Exception ex)
                {
                    await LoggingService.LogAsync($"{ex.Message}", TipoCodigo.ERROR);
                }

            }
        }
    }
}
