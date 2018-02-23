using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSports.Framework.Models.CustomClasses;

namespace LSports.Services.ViewModels
{
    public class ArrivalMessageViewModel
    {
        public ArrivalMessage ArrivalMessage { get; set; }
        public int? EventsCount { get; set; }
        public int? LocationsCount { get; set; }
        public int? LeaguesCount { get; set; }
        public int? SportsCount { get; set; }
        public int? MarketsCount { get; set; }
        public int? ProvidersCount { get; set; }
        public int? BetsCount { get; set; }
        public int? OpenBetsCount { get; set; }
    }
}
