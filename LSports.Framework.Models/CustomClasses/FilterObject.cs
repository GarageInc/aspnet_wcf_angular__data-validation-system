using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class FilterObject
    {
        public FilterValue Sport { get; set; }
        public FilterValue Country { get; set; }
        public FilterValue League { get; set; }
        public FilterValue Event { get; set; }
        public FilterValue Market { get; set; }
        public FilterValue Provider { get; set; }
        public FilterValue EventStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class FilterValue
    {
        public long? Id { get; set; }
        public long? ExternalId { get; set; }
        public string Name { get; set; }
    }
}