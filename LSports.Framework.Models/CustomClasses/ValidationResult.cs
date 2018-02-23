using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class ValidationResult
    {
        public int Id { get; set; }
        public int ValidationSettingId { get; set; }
        public long EventId { get; set; }
        public string EventName { get; set; }
        public string SportName { get; set; }
        public string LeagueName { get; set; }
        public string LocationName { get; set; }
        public string Market { get; set; }
        public string Provider { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdate { get; set; }
        public string XmlMessage { get; set; }
        public int ProductId { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public int Counter { get; set; }

        public virtual ValidationSetting ValidationSetting { get; set; }
        public long LeagueId { get; set; }
        public long LocationId { get; set; }
        public long SportId { get; set; }

        public string PointsToHighline { get; set; }
    }
}
