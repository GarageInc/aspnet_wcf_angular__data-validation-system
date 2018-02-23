using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class EventHistory
    {
        public int Id { get; set; }
        public long? EventId { get; set; }

        public string XmlText { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual Event Event { get; set; }
    }
}