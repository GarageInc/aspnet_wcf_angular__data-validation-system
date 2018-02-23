using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class RuleResult
    {
        public bool IsIssue { get; set; }

        public long? RuleId { get; set; }
        public string Market { get; set; }
        public  string Provider { get; set; }

        public string OuterXmls { get; set; }
        public List<string> PointsToHighline { get; set; } 
    }
}
