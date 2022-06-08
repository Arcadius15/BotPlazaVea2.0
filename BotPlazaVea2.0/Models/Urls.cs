using BotPlazaVea2._0.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotPlazaVea2._0.Models
{
    public class Urls
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string url { get; set; }
        public int pagina { get; set; }
        public string endpoint { get; set; }
        [EnumDataType(typeof(Status))]
        public Status status { get; set; }  


        public virtual Productos Producto { get; set; }
    }
}
