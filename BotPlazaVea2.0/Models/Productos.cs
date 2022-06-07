using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotPlazaVea2._0.Models
{
    public class Productos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string nombreProducto { get; set; }
        public decimal precioReg { get; set; }
        public decimal precioOferta { get; set; }
        public string proveedor { get; set; }
        public string categoria { get; set; }
        public string subcategoria { get; set; }
        public string tipo { get; set; }
        public string subtipo { get; set; }
        public string imagenUrl { get; set; }
        
        public virtual ICollection<Caracteristicas> caracteristicas { get; set; }
        public virtual ICollection<Descripciones> descripciones { get; set; }

        public int? idUrl { get; set; }
        public virtual Urls Url { get; set; }
    }
}
