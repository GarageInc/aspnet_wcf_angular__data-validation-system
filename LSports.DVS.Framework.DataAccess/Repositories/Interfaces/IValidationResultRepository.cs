using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.Framework.Models.CustomClasses;
using LSports.DVS.Framework.DataAccess.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories.Interfaces
{
    public interface IValidationResultRepository
    {
        IList<ValidationResult> GetValidationResultsByBorderDate(DateTime date);

        ValidationResult Get(int resultId);
        List<int> DisableForSettingEvent(int productId, int settingId, int eventId);

        void DisableOldValidationResults(IList<Event> events);

        List<ValidationResult> GetLastValidationResults(string validationHashIndex, IList<ValidationSetting> settings );

        void BulkInsertToPull(List<ValidationResultsHashModel> validationResults, string hashValidationIndex);
        void BulkInsertToPush(List<ValidationResultsHashModel> validationResults, string hashValidationIndex);

        void SetDisabledBySetting(int settingId);

        int GetNumberOfResultsByProductId(int productId);

        Dictionary<string, int> GetHistoricalDataByProductIdAndTimeFrame(int productId, DateTime from, DateTime to);

        int GetNumberOfResultsBySettingId(int productId, int settingId);

        IList<ValidationResult> GetResultsBySettingId(int settingId, int productId, int offset, int count, FilterObject filter, bool loadInactive);

        ValidationResultsStatistic GetFilteredResultsByProductId(int? productId, FilterObject filter, int settingId);

        // void DisablePreviousErrors(List<val_validationresult> results);
        
        val_validationresult NewRepositoryValidationResultFrom(RuleResultDTO ruleResult, ValidationSetting setting);

        // val_validationresult GetNewValResultsFromExisting();

        ValidationResultsHashModel NewRepositoryValidationResultFrom(ArrayList resultsWithoutMarketAndProviderDTO,
            ValidationSetting setting, Event @event, string market, string provider);

        void NewRuleResultFrom(Event @event, XObject node, XElement validatedNode, ValidationRule rule,
            bool isIssue, PointToHighline pointToHighline, 
            string pathToParentNode,
            Dictionary<int, Dictionary<string, int>> currentNodesFoundedCount,
            ref List<RuleResultDTO> ruleResults);

        DropDownValues GetDropDownValues(int? productId, int filterType, FilterValue selectedFilterValue);
    }
}
