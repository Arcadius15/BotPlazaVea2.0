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
                        proveedor = item.proveedor
                    };
                    url.Producto = p;
                    listaurls.Add(url);

                }

                try
                {
                    //bulk es para operaciones grandes, pero funciona correctamente para mapear con otros objetos
                    await context.Urls.AddRangeAsync(listaurls);
                    await context.BulkSaveChangesAsync();
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
                    await LoggingService.LogAsync($"{ex}", TipoCodigo.ERROR);
                }

            }
        }
    }
}
