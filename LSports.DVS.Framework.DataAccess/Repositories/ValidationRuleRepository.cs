using System.Collections.Generic;
using System.Linq;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using System;
using System.Data.Entity;
using LSports.Framework.Models.Enums;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class ValidationRuleRepository : IValidationRuleRepository
    {

        public IList<ValidationRule> List()
        {
            using (gb_dvsstagingEntities model = new gb_dvsstagingEntities())
            {
                var q = model.val_validationrule
                    .Where(rec => rec.IsActive && rec.ParameterId != null && rec.OperatorId != null)
                    .OrderBy(rec => rec.CreatedOn)
                    .ToList()
                    .Select(ValidationRuleFrom);

                return q.ToList();
            }
        }

        public static ValidationRule ValidationRuleFrom(val_validationrule rule)
        {
            var valRule =  new ValidationRule()
            {
                Id = rule.Id,

                OperatorId = rule.OperatorId,
                ParameterId = rule.ParameterId,
                DataTypeId = rule.DataTypeId,

                NumberOfChanges = rule.NumberOfChanges,

                PropertyName = rule.PropertyName,
                PropertyXPath = rule.PropertyXPath,

                Value = rule.Value,
                ValueXPath = rule.ValueXPath,

                RuleName = rule.RuleName,

                IsTime = rule.IsTime,

                IsForAllNodes = rule.IsForAllNodes,

                ValidationSettingId = rule.ValidationSettingId,
            };

            valRule.IsForAllNodes = rule.IsForAllNodes;

            return valRule;
        }

        public IList<ValidationRule> ListByParameters(List<int?> paramIds)
        {
            using (gb_dvsstagingEntities model = new gb_dvsstagingEntities())
            {
//                var rules = new List<ValidationRule>();

                var list = List()
                    .Where(rule=>paramIds.Contains(rule.ParameterId))
                    .ToList();

                return list;
            }
        }

        public int Insert(ValidationRule validationRule)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                val_validationrule val_Rule = new val_validationrule()
                {
                    ValidationSettingId = validationRule.ValidationSettingId,

                    RuleName = validationRule.RuleName,
                    PropertyName = validationRule.PropertyName,
                    PropertyXPath = validationRule.PropertyName.Replace(".", "/"),

                    ParameterId = validationRule.ParameterId,
                    
                    OperatorId = validationRule.OperatorId,
                    
                    CreatedBy = "Admin",
                    CreatedOn = DateTime.UtcNow,

                    IsTime = validationRule.IsTime,

                    IsForAllNodes = validationRule.IsForAllNodes,

                    IsActive = true
                };

                SetValueByParameter(validationRule, val_Rule);

                model.val_validationrule.Add(val_Rule);

                model.SaveChanges();

                return val_Rule.Id;
            }
        }

        public int Update(ValidationRule validationRule)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                val_validationrule val_Rule = model.val_validationrule.First(x => x.Id == validationRule.Id);

                val_Rule.ValidationSettingId = validationRule.ValidationSettingId;

                val_Rule.RuleName = validationRule.RuleName;
                val_Rule.PropertyName = validationRule.PropertyName;
                val_Rule.PropertyXPath = validationRule.PropertyName.Replace(".", "/");

                val_Rule.OperatorId = validationRule.OperatorId;

                val_Rule.ParameterId = validationRule.ParameterId;
                
                val_Rule.UpdatedBy = "Admin";
                val_Rule.UpdatedOn = DateTime.UtcNow;

                val_Rule.IsTime = validationRule.IsTime;

                val_Rule.IsForAllNodes = validationRule.IsForAllNodes;

                SetValueByParameter(validationRule, val_Rule);

                model.Entry(val_Rule).State = EntityState.Modified;

                model.SaveChanges();

                return val_Rule.Id;
            }
        }

        public void SetValueByParameter(ValidationRule from, val_validationrule to)
        {

            switch (to.ParameterId)
            {
                case (int)ValidationParameterId.Value:
                    {
                        to.Value = from.Value ?? "";
                        to.ValueXPath = (from.Value ?? "").Replace(".", "/");
                        break;
                    }
                case (int)ValidationParameterId.Length:
                    {
                        to.Value = from.Value;
                        to.ValueXPath = "";
                        break;
                    }
                case (int)ValidationParameterId.NumberOfChanges:
                    {
                        to.Value = from.Value;
                        to.NumberOfChanges = from.NumberOfChanges;
                        break;
                    }
                case (int)ValidationParameterId.NumberOfDistinctChanges:
                    {
                        to.Value = from.Value;
                        to.NumberOfChanges = from.NumberOfChanges;
                        break;
                    }
                case (int)ValidationParameterId.DataType:
                    {
                        to.DataTypeId = from.DataTypeId;
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public void Delete(int id)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var setting = model.val_validationrule.FirstOrDefault(s => s.Id == id);

                if (setting == null)
                    return;

                setting.IsActive = false;
                setting.UpdatedOn = DateTime.UtcNow;
                setting.UpdatedBy = "Admin";

                model.Entry(setting).State = EntityState.Modified;

                model.SaveChanges();
            }
        }
    }
}
