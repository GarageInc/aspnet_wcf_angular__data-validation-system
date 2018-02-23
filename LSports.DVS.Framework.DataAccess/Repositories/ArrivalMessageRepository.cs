using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using System;
using System.Data.Entity.Infrastructure;
using LSports.Framework.Models.Extensions;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class ArrivalMessageRepository : IArrivalMessageRepository
    {
        protected const int CONNECTION_TIMEOUT = 300;

        protected Object thisLock = new Object();

        public void BulkInsert(List<ArrivalMessage> messages, string groupId)
        {
            lock (thisLock)
            {
                using (var model = new gb_dvsstagingEntities())
                {
                    var repositoryMessages = messages.Select(x=>NewRepositoryArrivalMessage(x, groupId));

                    model.dvs_arrivalmessage.AddRange(repositoryMessages);

                    model.SaveChanges();
                }
            }
        }

        public int Insert(ArrivalMessage arrivalMessage)
        {
            lock (thisLock)
            {
                using (var model = new gb_dvsstagingEntities())
                {
                    var message = NewRepositoryArrivalMessage(arrivalMessage, "");

                    model.dvs_arrivalmessage.Add(message);
                    model.SaveChanges();

                    return message.Id;
                }
            }
        }

        public dvs_arrivalmessage NewRepositoryArrivalMessage(ArrivalMessage arrivalMessage, string groupId)
        {
            return new dvs_arrivalmessage
            {
                Events = string.Join(",", arrivalMessage.Events),
                Leagues = string.Join(",", arrivalMessage.Leagues),
                Locations = string.Join(",", arrivalMessage.Locations),
                Markets = string.Join(",", arrivalMessage.Markets),
                Providers = string.Join(",", arrivalMessage.Providers),
                Sports = string.Join(",", arrivalMessage.Sports),
                Statuses = string.Join(",", arrivalMessage.Statuses),
                Bets = string.Join(",", arrivalMessage.Bets),

                EventsCount = arrivalMessage.EventsCount,
                SportsCount = arrivalMessage.SportsCount,
                LocationsCount = arrivalMessage.LocationsCount,
                LeaguesCount = arrivalMessage.LeaguesCount,
                BetsCount = arrivalMessage.BetsCount,
                StatusesCount = arrivalMessage.StatusesCount,
                MarketsCount = arrivalMessage.MarketsCount,
                ProvidersCount = arrivalMessage.ProvidersCount,

                ProductId = arrivalMessage.ProductId,

                // LastUpdate = arrivalMessage.LastUpdate,

                CreatedOn = arrivalMessage.CreatedOn,
                CreatedBy = arrivalMessage.CreatedBy ?? "Admin",

                UpdatedBy = arrivalMessage.UpdatedBy,
                UpdatedOn = arrivalMessage.UpdatedOn,

                Url = arrivalMessage.Url,
                PathToXmlFile = arrivalMessage.PathToXmlFile,

                GroupId = arrivalMessage.GroupId,

                IsProcessed = arrivalMessage.IsProcessed,

                IsActive = true
            };
        }

        public void BulkUpdate(IList<ArrivalMessage> arrivalMessages)
        {
            lock (thisLock)
            {
                using (var model = new gb_dvsstagingEntities())
                {
                    model.Database.CommandTimeout = 300;

                    foreach (ArrivalMessage arrivaleMessageIterator in arrivalMessages)
                    {
                        var val_message = model.dvs_arrivalmessage.Find(arrivaleMessageIterator.Id, arrivaleMessageIterator.ProductId);

                        if (val_message != null)
                        {
                            var arrivalMessage = arrivaleMessageIterator;

                            CopyFromTo(ref arrivalMessage, ref val_message);

                            model.Entry(val_message).State = EntityState.Modified;
                        }
                    }

                    model.SaveChanges();
                }
            }
        }

        public void Update(ArrivalMessage arrivalMessage)
        {
            lock (thisLock)
            {
                using (var model = new gb_dvsstagingEntities())
                {
                    var val_message = model.dvs_arrivalmessage.Find(arrivalMessage.Id, arrivalMessage.ProductId);

                    CopyFromTo(ref arrivalMessage, ref val_message);

                    model.Entry(val_message).State = EntityState.Modified;

                    model.SaveChanges();
                }
            }
        }

        public void CopyFromTo(ref ArrivalMessage from, ref dvs_arrivalmessage to)
        {
            if (from.Events == null)
            {
                from.Events = new List<string>();
            }

            if (from.Leagues == null)
            {
                from.Leagues = new List<string>();
            }

            if (from.Locations == null)
            {
                from.Locations = new List<string>();
            }

            if (from.Markets == null)
            {
                from.Markets = new List<string>();
            }

            if (from.Providers == null)
            {
                from.Providers = new List<string>();
            }

            if (from.Sports == null)
            {
                from.Sports = new List<string>();
            }

            if (from.Statuses == null)
            {
                from.Statuses = new List<string>();
            }

            if (from.Bets == null)
            {
                from.Bets = new List<string>();
            }

            to.Events = string.Join(",", from.Events);
            to.Leagues = string.Join(",", from.Leagues);
            to.Locations = string.Join(",", from.Locations);
            to.Markets = string.Join(",", from.Markets);
            to.Providers = string.Join(",", from.Providers);
            to.Sports = string.Join(",", from.Sports);
            to.Statuses = string.Join(",", from.Statuses);
            to.Bets = string.Join(",", from.Bets);

            to.EventsCount = from.EventsCount;
            to.SportsCount = from.SportsCount;
            to.LocationsCount = from.LocationsCount;
            to.LeaguesCount = from.LeaguesCount;
            to.BetsCount = from.BetsCount;
            to.StatusesCount = from.StatusesCount;
            to.MarketsCount = from.MarketsCount;
            to.ProvidersCount = from.ProvidersCount;

            // val_message.ProductId = arrivalMessage.ProductId;
            //                        val_message.XmlMessage = arrivalMessage.XmlMessage;
            // val_message.LastUpdate = arrivalMessage.LastUpdate;
            //                            val_message.UpdatedOn = DateTime.UtcNow;

            to.IsProcessed = from.IsProcessed;
            to.PathToXmlFile = from.PathToXmlFile;
        }

        public IList<ArrivalMessage> GetNotProcessedMessages(List<int> productIds, int? limitCount)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var q = model.dvs_arrivalmessage.Where(x => x.IsActive
                                                            && x.IsProcessed == false
                                                            && productIds.Contains(x.ProductId));

                if (limitCount.HasValue)
                {
                    q = q.Take(limitCount.Value);
                }

                ///
                return q.ToList().Select(arrivalmessage => new ArrivalMessage
                         {
                             Id = arrivalmessage.Id,
                             Events = arrivalmessage.Events?.Split(',').ToList(),
                             Leagues = arrivalmessage.Leagues?.Split(',').ToList(),
                             Locations = arrivalmessage.Locations?.Split(',').ToList(),
                             Markets = arrivalmessage.Markets?.Split(',').ToList(),
                             Providers = arrivalmessage.Providers?.Split(',').ToList(),
                             Sports = arrivalmessage.Sports?.Split(',').ToList(),
                             Statuses = arrivalmessage.Statuses?.Split(',').ToList(),
                             Bets = arrivalmessage.Bets?.Split(',').ToList(),
                             ProductId = arrivalmessage.ProductId,
                             // XmlMessage = arrivalmessage.XmlMessage,
                             // LastUpdate = arrivalmessage.LastUpdate,
                             CreatedOn = arrivalmessage.CreatedOn,
                             IsProcessed = arrivalmessage.IsProcessed,
                             PathToXmlFile = arrivalmessage.PathToXmlFile,

                             GroupId = arrivalmessage.GroupId
                         }).ToList();
            }
        }

        public List<ArrivalMessage> GetLastArrivalMessageDateForProduct(int productId)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                model.Database.CommandTimeout = CONNECTION_TIMEOUT;

                var lastArrivalMessages = model.dvs_arrivalmessage
                    .Where(x => x.ProductId == productId && x.IsActive && !x.IsProcessed)
                    .OrderByDescending(t=>t.CreatedOn)
                    .Select(x=> new ArrivalMessage()
                    {
                        CreatedOn = x.CreatedOn
                    });

                return lastArrivalMessages.ToList();//.FirstOrDefault()?.CreatedOn ?? DateTime.UtcNow.AddDays(-1);
            }
        }

        public Dictionary<string, ArrivalMessage> GetChartDataForProduct(int productId, FilterObject filter)
        {
            if (filter == null)
                filter = new FilterObject();

            using (var dvs_model = new gb_dvsstagingEntities())
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

                dvs_model.Database.CommandTimeout = CONNECTION_TIMEOUT;

                var q = dvs_model.dvs_arrivalmessage.AsNoTracking()
                    .Where(result => result.ProductId == productId
                                    && result.IsProcessed && result.IsActive
                                    && (filter.StartDate == null || result.CreatedOn >= filter.StartDate)
                                    && (filter.EndDate == null || result.CreatedOn <= filter.EndDate)
                                    && (!filter.Sport.ExternalId.HasValue || result.Sports.Contains(",") && result.Sports.Contains("," + filter.Sport.ExternalId) || result.Sports.Contains(filter.Sport.ExternalId.ToString()))
                                    && (!filter.Event.ExternalId.HasValue || result.Events.Contains(",") && result.Events.Contains("," + filter.Event.ExternalId) || result.Events.Contains(filter.Event.ExternalId.ToString()))
                                    && (!filter.Country.ExternalId.HasValue || result.Locations.Contains(",") && result.Locations.Contains("," + filter.Country.ExternalId) || result.Locations.Contains(filter.Country.ExternalId.ToString()))
                                    && (string.IsNullOrEmpty(filter.EventStatus.Name) || result.Statuses.Contains(",") && result.Statuses.Contains("," + filter.EventStatus.Name) || result.Statuses.Contains(filter.EventStatus.Name))
                                    && (!filter.League.ExternalId.HasValue || (result.Leagues.Contains(",") && result.Leagues.Contains("," + filter.League.ExternalId) || result.Leagues.Contains(filter.League.ExternalId.ToString()))
                                    && (string.IsNullOrEmpty(filter.Market.Name) || result.Markets.Contains(",") && result.Markets.Contains("," + filter.Market.Name) || result.Markets.Contains(filter.Market.Name))
                                    && (string.IsNullOrEmpty(filter.Provider.Name) || result.Markets.Contains(",") && result.Providers.Contains("," + filter.Provider.Name) || result.Markets.Contains(filter.Provider.Name))))
                            .OrderBy(message => message.CreatedOn)
                            .ToList();

                var arrivalMessages = q
                    .Select(result => new ArrivalMessage
                    {
                        Id = result.Id,

                        Url = result.Url,

                        EventsCount = result.EventsCount,
                        LeaguesCount = result.LeaguesCount,
                        LocationsCount = result.LocationsCount,
                        MarketsCount = result.MarketsCount,
                        ProvidersCount = result.ProvidersCount,
                        SportsCount = result.SportsCount,
                        StatusesCount = result.StatusesCount,
                        BetsCount = result.BetsCount,
                        CreatedOn = result.CreatedOn,

                        Events = result.Events?.Split(',').ToList(),
                        Bets = result.Bets?.Split(',').ToList(),
                        Leagues = result.Leagues?.Split(',').ToList(),
                        Locations = result.Locations?.Split(',').ToList(),
                        Markets = result.Markets?.Split(',').ToList(),
                        Providers = result.Providers?.Split(',').ToList(),
                        Sports = result.Sports?.Split(',').ToList(),
                        Statuses = result.Statuses?.Split(',').ToList()
                    }).ToList();

                var startDate = new DateTime(filter.StartDate.Year, filter.StartDate.Month, filter.StartDate.Day, 0, 0,
                    0);
                
                var endDate = new DateTime(filter.EndDate.Year, filter.EndDate.Month, filter.EndDate.Day, 23, 59,
                    59);

                endDate.AddDays(1);

                var nextDate = new DateTime(filter.StartDate.Year, filter.StartDate.Month, filter.StartDate.Day, 23, 59,
                    59);

                var dataAccumulator = new Dictionary<DateTime, ArrivalMessage>();

                for (var i = 0; i < arrivalMessages.Count && startDate <= endDate;)
                {
                    var model = arrivalMessages[i];

                    var createdOn = model.CreatedOn;

                    var inRange = createdOn >= startDate &&
                                  nextDate >= createdOn;

                    if (!dataAccumulator.ContainsKey(startDate))
                    {
                        dataAccumulator.Add(startDate, new ArrivalMessage()
                        {
                            Sports = new List<string>(),
                            Locations = new List<string>(),
                            Leagues = new List<string>(),
                            Events = new List<string>(),
                            Providers = new List<string>(),
                            Markets = new List<string>(),
                            Bets = new List<string>()
                        });
                    } // pass

                    if (!inRange && startDate <= createdOn)
                    {
                        startDate = startDate.AddDays(1);
                        nextDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 23, 59, 59);
                    }
                    else if (!inRange && createdOn <= startDate)
                    {
                        i++;
                    }
                    else if (inRange)
                    {

                        if (dataAccumulator.ContainsKey(startDate))
                        {
                            if (model.Sports == null)
                            {
                                model.Sports = new List<string>();
                            }
                            if (model.Locations == null)
                            {
                                model.Locations = new List<string>();
                            }
                            if (model.Leagues == null)
                            {
                                model.Leagues = new List<string>();
                            }
                            if (model.Events == null)
                            {
                                model.Events = new List<string>();
                            }
                            if (model.Markets == null)
                            {
                                model.Markets = new List<string>();
                            }
                            if (model.Providers == null)
                            {
                                model.Providers = new List<string>();
                            }
                            if (model.Bets == null)
                            {
                                model.Bets = new List<string>();
                            }

                            dataAccumulator[startDate].Sports.AddRange(model.Sports);
                            dataAccumulator[startDate].Locations.AddRange(model.Locations);
                            dataAccumulator[startDate].Leagues.AddRange(model.Leagues);
                            dataAccumulator[startDate].Events.AddRange(model.Events);

                            dataAccumulator[startDate].Markets.AddRange(model.Markets);
                            dataAccumulator[startDate].Providers.AddRange(model.Providers);
                            dataAccumulator[startDate].Bets.AddRange(model.Bets);
                        }
                        else
                        {

                            dataAccumulator[startDate] = new ArrivalMessage
                            {
                                Sports = model.Sports,
                                Locations = model.Locations,
                                Leagues = model.Leagues,
                                Events = model.Events,

                                Markets = model.Markets,
                                Providers = model.Providers,
                                Bets = model.Bets
                            };
                        }

                        i++;
                    } // pass
                }


                var newDataAccumulator = new Dictionary<string, ArrivalMessage>();

                foreach (var keyValue in dataAccumulator)
                {
                    newDataAccumulator.Add(keyValue.Key.ToString(), new ArrivalMessage()
                    {
                        SportsCount = dataAccumulator[keyValue.Key].Sports.Distinct().Count(),
                        LocationsCount = dataAccumulator[keyValue.Key].Locations.Distinct().Count(),
                        LeaguesCount = dataAccumulator[keyValue.Key].Leagues.Distinct().Count(),
                        EventsCount = dataAccumulator[keyValue.Key].Events.Distinct().Count(),

                        MarketsCount = dataAccumulator[keyValue.Key].Markets.Distinct().Count(),
                        ProvidersCount = dataAccumulator[keyValue.Key].Providers.Distinct().Count(),
                        BetsCount = dataAccumulator[keyValue.Key].Bets.Distinct().Count(),
                    });
                }

                return newDataAccumulator;
            }
        }

        public
            IList<ArrivalMessage> GetFilteredItemsByProductId(int productId, FilterObject filter, int offset, int count)
        {
            if (filter == null)
                filter = new FilterObject();

            using (var model = new gb_dvsstagingEntities())
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

                model.Database.CommandTimeout = CONNECTION_TIMEOUT;

                var q = model.dvs_arrivalmessage
                         .Where(result => result.ProductId == productId
                            && result.IsProcessed && result.IsActive
                            && (filter.StartDate == null || result.CreatedOn >= filter.StartDate)
                            && (filter.EndDate == null || result.CreatedOn <= filter.EndDate)
                            && (!filter.Sport.ExternalId.HasValue || result.Sports.Contains(",") && result.Sports.Contains("," + filter.Sport.ExternalId) || result.Sports.Contains(filter.Sport.ExternalId.ToString()))
                            && (!filter.Event.ExternalId.HasValue || result.Events.Contains(",") && result.Events.Contains("," + filter.Event.ExternalId) || result.Events.Contains(filter.Event.ExternalId.ToString()))
                            && (!filter.Country.ExternalId.HasValue || result.Locations.Contains(",") && result.Locations.Contains("," + filter.Country.ExternalId) || result.Locations.Contains(filter.Country.ExternalId.ToString()))
                            && (string.IsNullOrEmpty(filter.EventStatus.Name) || result.Statuses.Contains(",") && result.Statuses.Contains("," + filter.EventStatus.Name) || result.Statuses.Contains(filter.EventStatus.Name))
                            && (!filter.League.ExternalId.HasValue || (result.Leagues.Contains(",") && result.Leagues.Contains("," + filter.League.ExternalId) || result.Leagues.Contains(filter.League.ExternalId.ToString()))
                            && (string.IsNullOrEmpty(filter.Market.Name) || result.Markets.Contains(",") && result.Markets.Contains("," + filter.Market.Name) || result.Markets.Contains(filter.Market.Name))
                            && (string.IsNullOrEmpty(filter.Provider.Name) || result.Markets.Contains(",") && result.Providers.Contains("," + filter.Provider.Name) || result.Markets.Contains(filter.Provider.Name))));

                q = q.OrderBy(x => x.CreatedOn)
                    .Skip(offset)
                    .Take(count);

                return q.Select(result => new ArrivalMessage
                {
                    Id = result.Id,

                    Url = result.Url,

                    EventsCount = result.EventsCount,
                    LeaguesCount = result.LeaguesCount,
                    LocationsCount = result.LocationsCount,
                    MarketsCount = result.MarketsCount,
                    ProvidersCount = result.ProvidersCount,
                    SportsCount = result.SportsCount,
                    StatusesCount = result.StatusesCount,
                    BetsCount = result.BetsCount,
                    CreatedOn = result.CreatedOn,

                }).ToList();
                
            }
        }

        public int GetCountOfFilteredItemsByProductId(int productId, FilterObject filter)
        {
            if (filter == null)
                filter = new FilterObject();

            using (var model = new gb_dvsstagingEntities())
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

                model.Database.CommandTimeout = CONNECTION_TIMEOUT;

                var count = model.dvs_arrivalmessage.Count(result => result.ProductId == productId
                            && result.IsProcessed && result.IsActive
                            && (filter.StartDate == null || result.CreatedOn >= filter.StartDate)
                            && (filter.EndDate == null || result.CreatedOn <= filter.EndDate)
                            && (!filter.Sport.ExternalId.HasValue || result.Sports.Contains(",") && result.Sports.Contains("," + filter.Sport.ExternalId) || result.Sports.Contains(filter.Sport.ExternalId.ToString()))
                            && (!filter.Event.ExternalId.HasValue || result.Events.Contains(",") && result.Events.Contains("," + filter.Event.ExternalId) || result.Events.Contains(filter.Event.ExternalId.ToString()))
                            && (!filter.Country.ExternalId.HasValue || result.Locations.Contains(",") && result.Locations.Contains("," + filter.Country.ExternalId) || result.Locations.Contains(filter.Country.ExternalId.ToString()))
                            && (string.IsNullOrEmpty(filter.EventStatus.Name) || result.Statuses.Contains(",") && result.Statuses.Contains("," + filter.EventStatus.Name) || result.Statuses.Contains(filter.EventStatus.Name))
                            && (!filter.League.ExternalId.HasValue || (result.Leagues.Contains(",") && result.Leagues.Contains("," + filter.League.ExternalId) || result.Leagues.Contains(filter.League.ExternalId.ToString()))
                            && (string.IsNullOrEmpty(filter.Market.Name) || result.Markets.Contains(",") && result.Markets.Contains("," + filter.Market.Name) || result.Markets.Contains(filter.Market.Name))
                            && (string.IsNullOrEmpty(filter.Provider.Name) || result.Markets.Contains(",") && result.Providers.Contains("," + filter.Provider.Name) || result.Markets.Contains(filter.Provider.Name))));

                return count;
            }
        }
    }
}
