using System.Collections.Generic;
using LSports.Framework.Models.CustomClasses;
using LSports.DVS.Framework.DataAccess.Models;

namespace LSports.DVS.Framework.DataAccess.Repositories.Interfaces
{
    public interface IValidationSettingRepository
    {
        IList<ValidationSetting> GetList();
        IList<ValidationSetting> GetListForProducts(List<int> productsIds);
        IList<ValidationSetting> GetListForProduct(int? productId);

        int Insert(ValidationSetting validationSetting);

        int Update(ValidationSetting validationSetting);

        void Delete(int validationSettingId);

        IList<ValidationOperator> GetValidationOperators();

        IList<ValidationDataType> GetValidationDataTypes();

        IList<ValidationParameter> GetValidationParameters();

        IList<ValidationRule> GetValidationRules(int? settingId);

        void UpdateLastValidateAtFor(List<int> settingsIds);
    }
}
