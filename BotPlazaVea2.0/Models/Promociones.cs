using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotPlazaVea2._0.Models
{
    public class Promociones
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string condicion { get; set; }

        public int? productoId { get; set; }
        public virtual Productos productos { get; set; }
    }
}
