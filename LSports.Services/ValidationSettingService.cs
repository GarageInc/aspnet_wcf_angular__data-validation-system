using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.Services.Interfaces;
using LSports.Services.ViewModels;

namespace LSports.Services
{
    public class ValidationSettingService : IValidationSettingService
    {
        private readonly IValidationSettingRepository _validationSettingRepository;
        private readonly IValidationResultRepository _validationResultRepository;

        public ValidationSettingService() : this(new ValidationSettingRepository(), new ValidationResultRepository())
        {
        }

        public ValidationSettingService(IValidationSettingRepository validationSettingRepository, IValidationResultRepository validationResultRepository)
        {
            _validationSettingRepository = validationSettingRepository;
            _validationResultRepository = validationResultRepository;
        }
        
        public IList<ValidationSettingViewModel> GetValidationSettingsForProduct( int? productId, FilterObject filter)
        {
            if (filter.Event == null)
                filter.Event = new FilterValue();
            if (filter.Country == null)
                filter.Country = new FilterValue();
            if (filter.EventStatus == null)
                filter.EventStatus = new FilterValue();
            if (filter.League == null)
                filter.League = new FilterValue();
            if (filter.Market == null)
                filter.Market = new FilterValue();
            if (filter.Provider == null)
                filter.Provider = new FilterValue();
            if (filter.Sport == null)
                filter.Sport = new FilterValue();

            var settings = _validationSettingRepository.GetListForProduct( productId);
            
            var results = new ConcurrentBag<ValidationSettingViewModel>();

            var tasks = new List<Task>();
            
            foreach (var setting in settings)
            {
                var settingId = setting.Id*1;

                var task = Task.Factory.StartNew(() =>
                {
                    var statistic = _validationResultRepository.GetFilteredResultsByProductId(productId, filter, settingId);

                    var settingViewModel = new ValidationSettingViewModel
                    {
                        Setting = setting,
                        OpenIssues = statistic.OpenIssues,
                        TotalIssues = statistic.TotalIssues,
                        LastTimeShown = statistic.LastTimeShown
                    };

                    results.Add(settingViewModel);
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            return results.ToList();
        }
    }
}