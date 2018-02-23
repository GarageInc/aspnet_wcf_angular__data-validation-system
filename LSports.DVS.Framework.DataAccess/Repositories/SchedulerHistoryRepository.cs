using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using System;
using LSports.Framework.Models.Enums;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class SchedulerHistoryRepository : ISchedulerHistoryRepository
    {
        protected const int CONNECTION_TIMEOUT = 300;

        public void Insert(SchedulerHistory history)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var newHistory = new dvs_schedulershistory
                {
                    CreatedBy = history.CreatedBy,
                    CreatedOn = history.CreatedOn,

                    StartedOn = history.StartedOn,
                    FinishedOn = history.FinishedOn,

                    IsByError = history.IsByError,
                    ErrorMessage = history.ErrorMessage,

                    AdditionalInfo = history.AdditionalInfo,

                    ArrivalMessageId = history.ArrivalMessageId,
                    ProductId = history.ProductId,

                    Type = history.Type,

                    IsActive = true
                };

                model.dvs_schedulershistory.Add(newHistory);
                model.SaveChanges();
            }
        }


        public Dictionary<string, List<SchedulerHistoryData>> GetHistoryByProductId(int productId, DateTime dateFrom, DateTime dateTo)
        {
            //dateFrom = dateFrom.Date + new TimeSpan(0, 0, 0);
            //dateTo = dateTo.Date + new TimeSpan(23, 59, 59);

            using ( var model = new gb_dvsstagingEntities())
            {
                model.Database.CommandTimeout = CONNECTION_TIMEOUT;

                var creations = model.dvs_schedulershistory
                    .Where(res => res.ProductId == productId && res.IsActive && res.CreatedOn >= dateFrom && res.CreatedOn <= dateTo)
                    .Where(res =>  res.Type == (int)SchedulerTypes.ParseXmlFile 
                                || res.Type == (int)SchedulerTypes.Downloader
                                || res.Type == (int)SchedulerTypes.EventsValidator
                                || res.Type == (int)SchedulerTypes.Merger
                    )
                    .OrderBy(res=>res.CreatedOn)
                    .ToList();
                
                var result = new List<SchedulerHistoryData>();

                foreach(var creation in creations)
                {
                    // var fromDate = dateFrom.Date;

                    result.Add(new SchedulerHistoryData
                    {
                        date = creation.CreatedOn.ToString("yyyy-MM-dd HH:mm"),
                        timeframe = (creation.FinishedOn - creation.StartedOn)?.TotalSeconds,
                        type = creation.Type.ToString(),
                        isByError = creation.IsByError,
                        errorMessage = creation.ErrorMessage,
                        additionalInfo = creation.AdditionalInfo
                    });
                }

                var groupedByType = result.GroupBy(x => x.type);
                var dict = new Dictionary<string, List<SchedulerHistoryData>>();

                foreach (var groupByType in groupedByType)
                {
                    var key = groupByType.Key.ToString();
                    dict.Add(key, new List<SchedulerHistoryData>());

                    var groupedByTime = groupByType.GroupBy(x => x.date);
                    
                    foreach (var groupByTime in groupedByTime)
                    {
                        var isByError = groupByTime.Any(x => x.isByError);

                        var values = groupByTime.ToList();

                        dict[key].Add(new SchedulerHistoryData()
                        {
                            timeframe = values.Sum(x=>x.timeframe)/ values.Count,
                            type = key,
                            date = groupByTime.Key,
                            additionalInfo = !isByError ? string.Join("<br>+", values.Select(x=>x.additionalInfo)) : "",
                            errorMessage = isByError ? string.Join("<br>+", values.Select(x => x.errorMessage)) : "",
                            isByError = isByError
                        });
                    }
                }
                
                return dict;
            }
        }

        public IList<Object> GetHistoryOfValidation(DateTime dateFrom, DateTime dateTo)
        {
            //dateFrom = dateFrom.Date + new TimeSpan(0, 0, 0);
            //dateTo = dateTo.Date + new TimeSpan(23, 59, 59);

            using (var model = new gb_dvsstagingEntities())
            {
                model.Database.CommandTimeout = CONNECTION_TIMEOUT;

                var creations = model.dvs_schedulershistory
                    .Where(res => res.IsActive && res.CreatedOn >= dateFrom && res.CreatedOn <= dateTo)
                    .Where(res => res.Type == (int)SchedulerTypes.ParseXmlFile
                                || res.Type == (int)SchedulerTypes.Downloader
                                || res.Type == (int)SchedulerTypes.EventsValidator
                                || res.Type == (int)SchedulerTypes.Merger
                    )
                    .OrderBy(res => res.CreatedOn)
                    .ToList();

                var result = new List<Object>();

                foreach (var creation in creations)
                {

                    var date = creation.CreatedOn.ToString("yyyy-MM-dd HH:mm");
                    result.Add(new
                    {
                        date = date,
                        timeframe = (creation.FinishedOn - creation.StartedOn)?.TotalSeconds,
                        type = creation.Type,
                        isByError = creation.IsByError,
                        errorMessage = creation.ErrorMessage
                    });
                }

                return result;
            }
        }
    }

    public class SchedulerHistoryData
    {
        public string date { get; set; }
        public double? timeframe { get; set; }
        public string type { get; set; }
        public bool isByError { get; set; }
        public string errorMessage { get; set; }
        public string additionalInfo { get; set; }
    }
}
