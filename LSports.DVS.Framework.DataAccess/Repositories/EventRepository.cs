using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.Framework.Models.Extensions;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class EventRepository : IEventRepository
    {
        private const string Admin = "Admin";
        protected const int CONNECTION_TIMEOUT = 300;
        protected const int INSERT_COUNT = 200;// if bigger, then apper error of max_packet_size
        protected Object thisLock = new Object();

        public IList<Event> GetList()
        {
            using (var model = new gb_dvsstagingEntities())
            {
                model.Database.CommandTimeout = CONNECTION_TIMEOUT;

                var q = model.dvs_event
                    .Where(@event => @event.IsActive)
                    .ToList()
                    .Select(NewEventFrom);

                return q.ToList();
            }
        }

        public List<Event> GetEventsByEventIds(List<long> eventIds, int productId)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                model.Configuration.AutoDetectChangesEnabled = false;
                model.Configuration.ValidateOnSaveEnabled = false;
                model.Database.CommandTimeout = CONNECTION_TIMEOUT;

                var events = model.dvs_event
                    .Where(x => x.ProductId == productId && eventIds.Contains(x.EventId))
                    .ToList()
                    .Select(NewEventFrom)
                    .ToList();

                return events;
            }
        }


        public int GetCountForProduct(int productId)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var count = model.dvs_event
                    
                    .Count(@event => @event.IsActive && @event.ProductId == productId);

                return count;
            }
        }
        public IList<Event> GetListByOffsetAndCountForProduct(int productId, int offset, int count)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                model.Configuration.AutoDetectChangesEnabled = false;
                model.Configuration.ValidateOnSaveEnabled = false;
                model.Database.CommandTimeout = CONNECTION_TIMEOUT;

                var actives = model.dvs_event
                    .AsNoTracking()
                    .Where(@event => @event.IsActive && @event.ProductId == productId)
                    .OrderBy(@event => @event.CreatedOn);

                var q = actives
                    .Skip(offset)
                    
                    .Take(count)
                    .ToList()
                    .Select(NewEventFrom);

                return q.ToList();
            }
        }

        public Event Insert(Event @event)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var newRecord = NewRepositoryEventFrom(@event);

                model.dvs_event.Add(newRecord);
                model.SaveChanges();

                @event.Id = newRecord.Id;

                return @event;
            }
        }

        public void BulkInsertByCreatingHistories(IList<Event> inserting)
        {
            lock (this.thisLock)
            {
                List<long> eventsIds = GetListEventsIds();

                using (var model = new gb_dvsstagingEntities())
                {
                    model.Configuration.AutoDetectChangesEnabled = false;
                    model.Configuration.ValidateOnSaveEnabled = false;
                    model.Database.CommandTimeout = CONNECTION_TIMEOUT;


                    // will add by counter events
                    for (int i = 0; i <= inserting.Count() / INSERT_COUNT; i++)
                    {
                        var events = new List<dvs_event>();
                        var histories = new List<dvs_eventhistory>();

                        var subEvents = inserting.Skip(i * INSERT_COUNT).Take(INSERT_COUNT).ToList();

                        foreach (var @event in subEvents)
                        {
                            if (eventsIds.Any(eventId => @event.EventId == eventId))
                            {
                                // pass
                            }
                            else
                            {
                                events.Add(NewRepositoryEventFrom(@event));
                            }

                            histories.Add(EventHistoryRepository.NewRepositoryEventHistoryFrom(@event));
                        }

                        model.dvs_event.AddRange(events);
                        model.dvs_eventhistory.AddRange(histories);
                        model.SaveChanges();
                    }
                }
            }
        }

        public void BulkInsert(IList<Event> events)
        {
            // lock (this.thisLock)
            {
                using (var dbConnection = new DbConnection.DbConnection())
                {
                    var updatedOn = DateTime.UtcNow.ToString("yy-MM-dd HH:mm:ss");

                    for (int i = 0; i <= events.Count / INSERT_COUNT; i++)
                    {
                        var subEvents = events.Skip(i * INSERT_COUNT).Take(INSERT_COUNT).ToList();

                        if (subEvents.Any())
                        {
                            var query = new
                                StringBuilder(
                                    "INSERT DELAYED INTO dvs_event (EventId,EventName,StartDate,SportId,SportName,LeagueId, " +
                                    "LeagueName,LocationId,LocationName,Status,LastUpdate,HomeTeamId,HomeTeamName,AwayTeamId,AwayTeamName," +
                                    "CreatedOn,CreatedBy,IsActive,UpdatedOn,UpdatedBy,MarketId,MarketName,ProviderId,ProviderName,ProductId) VALUES ");

                            var counter = 0;

                            foreach (var @event in subEvents)
                            {
                                //@event.XmlText = "blabla";

                                query.Append("(");
                                query.Append(@event.EventId);
                                query.Append(",'" + @event.BuildEventName() + "'");
                                query.Append(",'" + @event.StartDate + "'");
                                query.Append(",'" + (@event.SportId ?? 0) + "'");
                                query.Append(",'" + @event.SportName + "'");
                                query.Append(",'" + (@event.LeagueId ?? 0) + "'");
                                query.Append(",'" + @event.LeagueName + "'");
                                query.Append(",'" + (@event.LocationId ?? 0) + "'");
                                query.Append(",'" + @event.LocationName + "'");
                                query.Append(",'" + @event.Status + "'");
                                query.Append(",'" + @event.LastUpdate.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "'");
                                query.Append(",'" + (@event.HomeTeamId ?? 0) + "'");
                                query.Append(",'" + @event.HomeTeamName + "'");
                                query.Append(",'" + (@event.AwayTeamId ?? 0) + "'");
                                query.Append(",'" + @event.AwayTeamName + "'");
                                query.Append(",'" + @event.CreatedOn.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "'");
                                query.Append(",'" + @event.CreatedBy + "'");
                                query.Append(", b'1'");
                                query.Append(",'" + updatedOn + "'");
                                query.Append(",'" + @event.UpdatedBy + "'");
                                query.Append(",'" + (@event.MarketId ?? 0) + "'");
                                query.Append(",'" + @event.MarketName + "'");
                                query.Append(",'" + (@event.ProviderId ?? 0) + "'");
                                query.Append(",'" + @event.ProviderName + "'");
                                query.Append(",'" + @event.ProductId + "'");
                                query.Append(")");

                                if (counter != subEvents.Count - 1)
                                {
                                    query.Append(",");
                                }

                                counter++;
                            }

                            query.Append(" ON DUPLICATE KEY UPDATE");
                            query.Append(" UpdatedOn = '" + updatedOn + "'");
                            query.Append(";");

                            //throw  new Exception(query.ToString());
                            dbConnection.BulkExecute(query.ToString());
                        }// else pass
                    }
                }
            }
        }


        public List<long> GetListEventsIds()
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var ids = model.dvs_event
                    .Where(@event => @event.IsActive)
                    .Select(@event => @event.EventId)
                    .ToList();

                return ids;
            }
        }

        private static dvs_event NewRepositoryEventFrom(Event @event)
        {
            return new dvs_event
            {
                EventId = @event.EventId,
                EventName = @event.BuildEventName(),
                StartDate = @event.StartDate,
                SportId = @event.SportId,
                SportName = @event.SportName,
                LeagueId = @event.LeagueId,
                LeagueName = @event.LeagueName,
                LocationId = @event.LocationId,
                LocationName = @event.LocationName,
                Status = @event.Status,
                LastUpdate = @event.LastUpdate,
                HomeTeamId = @event.HomeTeamId,
                HomeTeamName = @event.HomeTeamName,
                AwayTeamId = @event.AwayTeamId,
                AwayTeamName = @event.AwayTeamName,
                // XmlText = @event.XmlTextExample,
                MarketId = @event.MarketId,
                MarketName = @event.MarketName,
                ProviderId = @event.ProviderId,
                ProviderName = @event.ProviderName,
                CreatedBy = Admin,
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = Admin,
                UpdatedOn = DateTime.UtcNow,
                IsActive = true,
                ProductId = @event.ProductId
            };
        }

        public static Event NewEventFrom(dvs_event @event)
        {
            return new Event
            {
                Id = @event.Id,
                EventId = @event.EventId,
                EventName = @event.EventName,
                AwayTeamId = @event.AwayTeamId,
                AwayTeamName = @event.AwayTeamName,
                HomeTeamId = @event.HomeTeamId,
                HomeTeamName = @event.HomeTeamName,
                LeagueId = @event.LeagueId,
                LeagueName = @event.LeagueName,
                LocationId = @event.LocationId,
                LocationName = @event.LocationName,
                SportId = @event.SportId,
                SportName = @event.SportName,
                Status = @event.Status,
                StartDate = @event.StartDate,
                LastUpdate = @event.LastUpdate,
                CreatedBy = Admin,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                ProductId = @event.ProductId
            };
        }

        public Event Update(Event department)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public bool IsEventNameUnique(string name, int id)
        {
            throw new System.NotImplementedException();
        }

        public Event GetItemByEventId(long eventId)
        {
            throw new System.NotImplementedException();
        }
    }
}