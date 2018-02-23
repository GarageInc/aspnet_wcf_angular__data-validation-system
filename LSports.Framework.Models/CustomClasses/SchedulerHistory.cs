using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class SchedulerHistory
    {
        public int Id { get; set; }
        public string AdditionalInfo { get; set; }

        public bool IsActive { get; set; }
        public bool IsByError { get; set; }
        public string ErrorMessage { get; set; }

        public System.DateTime StartedOn { get; set; }
        public System.DateTime FinishedOn { get; set; }

        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        
        public int? ProductId { get; set; }
        public int? ArrivalMessageId { get; set; }

        public int Type { get; set; }
        
        public virtual Product Product { get; set; }
        public virtual ArrivalMessage ArrivalMessage { get; set; }
    }
}
