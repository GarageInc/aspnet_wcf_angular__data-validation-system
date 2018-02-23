using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class HistoriesGroup
    {
        public long? EventId { get; set; }

        public string BaseEventXmlText { get; set; }

        public IList<EventHistory> EventHistories { get; set; }
    }
}
