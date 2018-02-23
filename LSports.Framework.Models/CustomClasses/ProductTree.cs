using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class ProductTree
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string XmlText { get; set; }

        public TreeCore Tree { get; set; }

        public virtual Product Product { get; set; }
    }
}
