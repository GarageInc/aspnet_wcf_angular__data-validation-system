using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class SubStatistic
    {
        public bool IsActive { get; set; }
        public DateTime LastTimeShown { get; set; }
    }

    public class ValidationResultsStatistic
    {
        public int TotalIssues { get; set; }
        public int OpenIssues { get; set; }
        public DateTime LastTimeShown { get; set; }
    }

}
