using System.Collections.Generic;

namespace LSports.Framework.Models.CustomClasses
{
    public class DropDownValues
    {
        public IList<FilterValue> Events { get; set; }
        public IList<FilterValue> Sports { get; set; }
        public IList<FilterValue> Countries { get; set; }
        public IList<FilterValue> Leagues { get; set; }
        public IList<FilterValue> Statuses { get; set; }
        public IList<FilterValue> Markets { get; set; }
        public IList<FilterValue> Providers { get; set; }
    }
}
