using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSports.Framework.Models.CustomClasses;

namespace LSports.Services.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public int NumberOfIssues { get; set; }
        public Dictionary<string, int> GraphData { get; set; }
    }
}
