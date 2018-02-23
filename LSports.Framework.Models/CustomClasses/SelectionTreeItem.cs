using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class SelectionTreeItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long? ExternalId { get; set; }
        public long? ParentId { get; set; }
        public int? TypeId { get; set; }
    }
}
