using System.Collections.Generic;
using LSports.Framework.Models.CustomClasses;
using LSports.Framework.Models.Enums;

namespace LSports.Framework.DataAccess.Repositories.Interfaces
{
    public interface IValidationRuleRepository
    {
        IList<ValidationRule> List();
        IList<ValidationRule> ListByParameters(List<int?> paramIds);

        int Insert(ValidationRule validationRule);
        int Update(ValidationRule validationRule);
        
        void Delete(int id);
    }
}
