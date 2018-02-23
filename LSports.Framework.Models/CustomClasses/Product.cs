using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GatewayAPI { get; set; }
        public bool? IsPushService { get; set; }
    }

    public class LastArrivalMessagesInfo
    {
        public DateTime LastDownloadedAt
        {
            get;
            set;
        }

        public int CountLastHour { get; set; }
    }
}
