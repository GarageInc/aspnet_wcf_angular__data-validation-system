using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LSports.Framework.Models.CustomClasses
{
    public class RuleResultDTO
    {
       // public Guid Id { get; set; }

        public bool IsIssue = true;

        public Event Event { get; set; }
        public ValidationRule Rule { get; set; }

        public bool IsMarket = true;
        public string Market = "";

        public bool IsProvider = true;
        public  string Provider = "";

        //public string OuterXml { get; set; }
        public PointToHighline PointToHighline { get; set; }

        public string ParentNodeName { get; set; }

        public XElement ParentXElement { get; set; }

        public XElement ParentValidatedXElement { get; set; }

    }
}
