using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSports.Framework.Models.CustomClasses;

namespace LSports.Services.ViewModels
{
    public class ValidationSettingViewModel
    {
        public ValidationSetting Setting { get; set; }

        public int OpenIssues { get; set; }

        public int TotalIssues { get; set; }

        public DateTime LastTimeShown { get; set; }
    }
}
