using System;
using System.Collections.Generic;
using System.Linq;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using System.Data.Entity;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class ValidationSettingRepository : IValidationSettingRepository
    {
        public void UpdateLastValidateAtFor(List<int> settingsIds)
        {

            using (var model = new gb_dvsstagingEntities())
            {
                var q = model.val_validationsetting.Where(x => settingsIds.Contains(x.Id));

                foreach (var valValidationsetting in q)
                {
                    valValidationsetting.LastValidateAt = DateTime.UtcNow;
                }

                model.SaveChanges();
                //return q;
            }
        }


        public IList<ValidationSetting> GetList()
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var q = (from setting in model.val_validationsetting
                         join priority in model.val_priority on setting.PriorityId equals priority.Id
                         join product in model.dvs_product on setting.ProductId equals product.Id
                         where setting.IsActive
                         select new ValidationSetting
                         {
                             Id = setting.Id,
                             Name = setting.Name,
                             Description = setting.Description,
                             ProductId = setting.ProductId,
                             PriorityId = setting.PriorityId,
                             Expression = setting.Expression,
                             LastValidatedAt = setting.LastValidateAt,
                             Priority = new Priority
                             {
                                 Id = priority.Id,
                                 Name = priority.Name
                             },
                             Product = new Product
                             {
                                 Id = product.Id,
                                 Name = product.Name
                             }
                         }).ToList();

                return q;
            }
        }

        public IList<ValidationSetting> GetListForProducts(List<int> productsIds){
            using (var model = new gb_dvsstagingEntities())
            {
                var q = (from setting in model.val_validationsetting
                         join priority in model.val_priority on setting.PriorityId equals priority.Id
                         join product in model.dvs_product on setting.ProductId equals product.Id
                         where productsIds.Contains((int)setting.ProductId)
                         where setting.IsActive
                         select new ValidationSetting
                         {
                             Id = setting.Id,
                             Name = setting.Name,
                             Description = setting.Description,
                             PriorityId = setting.PriorityId,
                             ProductId = setting.ProductId,
                             Expression = setting.Expression,
                             LastValidatedAt = setting.LastValidateAt,
                             Priority = new Priority
                             {
                                 Id = priority.Id,
                                 Name = priority.Name
                             },
                             Product = new Product
                             {
                                 Id = product.Id,
                                 Name = product.Name
                             },
                             IsSlackEnabled = setting.IsSlackEnabled,
                             SlackChannel = setting.SlackChannel
                         }).ToList();

                return q;
            }
        }


        public IList<ValidationSetting> GetListForProduct( int? productId)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var q = (from setting in model.val_validationsetting
                         join priority in model.val_priority on setting.PriorityId equals priority.Id
                         join product in model.dvs_product on setting.ProductId equals product.Id
                         where setting.IsActive
                         select new ValidationSetting
                         {
                             Id = setting.Id,
                             Name = setting.Name,
                             Description = setting.Description,
                             PriorityId = setting.PriorityId,
                             ProductId = setting.ProductId,
                             Expression = setting.Expression,
                             CreatedOn = setting.CreatedOn,
                             UpdatedOn = setting.UpdatedOn,
                             LastValidatedAt = setting.LastValidateAt,
                             Priority = new Priority
                             {
                                 Id = priority.Id,
                                 Name = priority.Name
                             },
                             Product = new Product
                             {
                                 Id = product.Id,
                                 Name = product.Name
                             },
                             IsSlackEnabled = setting.IsSlackEnabled,
                             SlackChannel = setting.SlackChannel
                         });

                if (productId.HasValue)
                {
                    q = q.Where(x => x.ProductId == productId);
                };

                return q.ToList();
            }
        }

        public int Insert(ValidationSetting validationSetting)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var val_valSetting = new val_validationsetting
                {
                    Name = validationSetting.Name,
                    Description = validationSetting.Description,

                    PriorityId = validationSetting.PriorityId,
                    ProductId = validationSetting.ProductId,
                    Expression = validationSetting.Expression,

                    CreatedBy = "Admin",
                    CreatedOn = DateTime.UtcNow,

                    UpdatedBy = "Admin",
                    UpdatedOn = DateTime.UtcNow,

                    IsSlackEnabled = validationSetting.IsSlackEnabled,
                    SlackChannel = validationSetting.SlackChannel,

                    IsActive = true                    
                };
                model.val_validationsetting.Add(val_valSetting);

                model.SaveChanges();

                return val_valSetting.Id;
            }
        }

        public int Update(ValidationSetting validationSetting)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                val_validationsetting val_Setting = model.val_validationsetting.First(x => x.Id == validationSetting.Id);
                
                val_Setting.Name = validationSetting.Name;
                val_Setting.Description = validationSetting.Description;

                val_Setting.PriorityId = validationSetting.PriorityId;
                val_Setting.ProductId = validationSetting.ProductId;
                val_Setting.Expression = validationSetting.Expression;

                val_Setting.UpdatedBy = "Admin";
                val_Setting.UpdatedOn = DateTime.UtcNow;

                val_Setting.IsSlackEnabled = validationSetting.IsSlackEnabled;
                val_Setting.SlackChannel = validationSetting.SlackChannel;

                model.Entry(val_Setting).State = EntityState.Modified;

                model.SaveChanges();

                return val_Setting.Id;
            }
        }

        public void Delete(int validationSettingId)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var setting = model.val_validationsetting.FirstOrDefault(s => s.Id == validationSettingId);
                if (setting == null)
                    return;

                setting.IsActive = false;
                setting.UpdatedOn = DateTime.UtcNow;
                setting.UpdatedBy = "Admin";

                model.SaveChanges();
            }
        }

        public IList<ValidationOperator> GetValidationOperators()
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var q = model.val_operator.Where(op => op.IsActive).Select(op => new ValidationOperator {Id = op.Id, Name = op.Name}).ToList();

                return q;
            }
        }


        public IList<ValidationDataType> GetValidationDataTypes()
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var q = model.val_datatypes
                    .Where(op => op.IsActive)
                    .Select(op => new ValidationDataType
                    {
                        Id = op.Id, Name = op.Name
                    })
                    .ToList();
                return q;
            }
        }


        public IList<ValidationParameter> GetValidationParameters()
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var q = model.val_parameter
                    .Where(par => par.IsActive)
                    .Select(parameter => new ValidationParameter {
                        Id = parameter.Id, Name = parameter.Name
                    })
                    .ToList();
                return q;
            }
        }

        public IList<ValidationRule> GetValidationRules(int? settingId)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var q = (from rule in model.val_validationrule
                    join setting in model.val_validationsetting on rule.ValidationSettingId equals setting.Id
                    join oper in model.val_operator on rule.OperatorId equals oper.Id
                    join parameter in model.val_parameter on rule.ParameterId equals parameter.Id
                    orderby rule.CreatedOn
                    where !settingId.HasValue || rule.ValidationSettingId == settingId
                    where setting.IsActive
                    where rule.IsActive
                    select new ValidationRule
                    {
                        Id = rule.Id,
                        RuleName = rule.RuleName,
                        OperatorId = rule.OperatorId,
                        ParameterId = rule.ParameterId,
                        DataTypeId = rule.DataTypeId,

                        NumberOfChanges = rule.NumberOfChanges,
                        
                        PropertyName = rule.PropertyName,
                        PropertyXPath = rule.PropertyXPath,

                        Value = rule.Value,
                        ValueXPath = rule.ValueXPath,

                        IsTime = rule.IsTime,

                        ValidationSettingId = rule.ValidationSettingId,

                        IsForAllNodes = rule.IsForAllNodes,

                        ValidationSetting = new ValidationSetting {Name = setting.Name, Id = setting.Id},
                        Operator = new ValidationOperator {Id = oper.Id, Name = oper.Name},
                        Parameter = new ValidationParameter {Id = parameter.Id, Name = parameter.Name}
                    }).ToList();

                return q;
            }
        }
    }
}