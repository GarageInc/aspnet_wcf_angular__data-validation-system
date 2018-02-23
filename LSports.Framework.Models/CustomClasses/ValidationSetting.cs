using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class ValidationSetting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? PriorityId { get; set; }
        public int? ProductId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public DateTime? LastValidatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string Expression { get; set; }

        public virtual Priority Priority { get; set; }
        public virtual Product Product { get; set; }

        public IList<ValidationRule> ValidationRules { get; set; }

        public bool IsContainsRuleForAllNodes = false;

        public bool IsContainsRelatedRules = false;

        public bool IsSlackEnabled { get; set; }
        public string SlackChannel { get; set; }
    }
}
