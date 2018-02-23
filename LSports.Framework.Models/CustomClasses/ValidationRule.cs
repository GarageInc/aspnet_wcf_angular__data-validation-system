using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class ValidationRule
    {
        public int Id { get; set; }
        public int? ValidationSettingId { get; set; }

        public string PropertyName { get; set; }
        public string PropertyXPath { get; set; }

        protected string PathToPropertyParentNodeProtected = null;
        protected bool IsSetPathToPropertyParentNode = false;
        public string PathToPropertyParentNode
        {
            get
            {
                if (PathToPropertyParentNodeProtected == null && !IsSetPathToPropertyParentNode)
                {
                    var lastIndex = PropertyXPath?.LastIndexOf("/", StringComparison.Ordinal);
                    PathToPropertyParentNodeProtected = (lastIndex != null && lastIndex > 0) ? PropertyXPath.Substring(0, lastIndex.Value) : ValueXPath;
                    IsSetPathToPropertyParentNode = true;
                }

                return PathToPropertyParentNodeProtected;
            }
        }

        protected string[] StorageForPropertyXPath = new string[0];
        public string[] PropertyXPathForValidator
        {
            get
            {
                if (StorageForPropertyXPath.Length == 0 && PropertyXPath != null)
                {
                    StorageForPropertyXPath = PropertyXPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                }

                return StorageForPropertyXPath;
            }
        }

        public int? ParameterId { get; set; }

        public int? OperatorId { get; set; }

        public int? DataTypeId { get; set; }

        public string Value { get; set; }
        public string ValueXPath { get; set; }


        protected string PathToValueParentNodeProtected = null;
        protected bool IsSetPathToValueParentNode = false;
        public string PathToValueParentNode
        {
            get
            {
                if (PathToValueParentNodeProtected == null && !IsSetPathToValueParentNode)
                {
                    var lastIndex = ValueXPath?.LastIndexOf("/", StringComparison.Ordinal);
                    PathToValueParentNodeProtected = (lastIndex != null && lastIndex > 0) ? ValueXPath.Substring(0, lastIndex.Value) : ValueXPath;
                    IsSetPathToValueParentNode = true;
                }

                return PathToValueParentNodeProtected;
            }
        }

        protected string[] StorageForValueXPath = new string[0];
        public string[] ValueXPathForValidator
        {
            get
            {
                if (StorageForValueXPath.Length == 0 && ValueXPath != null)
                {
                    StorageForValueXPath = ValueXPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                }

                return StorageForValueXPath;
            }
        }

        public bool IsOneParentNode => StorageForValueXPath.Length > 1 && StorageForPropertyXPath.Length > 1
                                       && StorageForValueXPath[StorageForValueXPath.Length - 2] == StorageForPropertyXPath[StorageForPropertyXPath.Length - 2];

        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public int? NumberOfChanges { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public string RuleName { get; set; }

        public bool? IsTime { get; set; }
        
        public bool IsForAllNodes { get; set; }

        public virtual ValidationOperator Operator { get; set; }
        public virtual ValidationParameter Parameter { get; set; }
        public virtual ValidationSetting ValidationSetting { get; set; }
        
        public ValidationRule()
        {
            IsForAllNodes = false;
        }
    }
}
