using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class EventHistoryRepository : IEventHistoryRepository
    {
        private const string Admin = "Admin";
        protected const int CONNECTION_TIMEOUT = 300;
        protected const int INSERT_COUNT = 2000;

        public IList<EventHistory> GetList()
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var q = model.dvs_eventhistory
                    .Where(history => history.IsActive)
                    .ToList()
                    .Select(NewEventHistoryFrom);

                return q.ToList();
            }
        }

        public int GetCount()
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var count = model.dvs_eventhistory
                    .Count(@event => @event.IsActive);

                return count;
            }
        }

        public int GetNotMergedCount()
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var count = model.dvs_eventhistory
                    .Count(@event => @event.IsActive && @event.IsMerged == false);

                return count;
            }
        }
        /*
        public async void SaveHistoriesGroups(List<HistoriesGroup> groups)
        {
            using(var model = new gb_dvsstagingEntities())
            {
                // comment, because not saved is merged
                //model.Configuration.AutoDetectChangesEnabled = false;
                //model.Configuration.ValidateOnSaveEnabled = false;
                model.Database.CommandTimeout = CONNECTION_TIMEOUT;

                var eventIds = groups.Select(x => x.EventId).ToList();
                var historiesIds = new List<int>();

                foreach(var g in groups)
                {
                    historiesIds.AddRange(g.EventHistories.Select(x => x.Id));
                }
                
                // save events xmls
                var eventsQuery = (from @event in model.dvs_event
                                  where @event.IsActive && eventIds.Contains(@event.EventId)
                                  select @event);

                var tasks = eventsQuery.ForEachAsync((@event) =>
                {
                    var group = groups.Find(x => x.EventId == @event.EventId);

                    if (group != null)
                    {
                        @event.XmlText = group.BaseEventXmlText;
                    }
                });

                await Task.WhenAll(tasks);

                await model.SaveChangesAsync();

                // save as merged
                var historiesQuery = (from history in model.dvs_eventhistory
                                      where history.IsActive && history.IsMerged == false && historiesIds.Contains(history.Id)
                                      select history);

                tasks = historiesQuery.ForEachAsync((history) =>
                {
                    history.IsActive = false;
                    history.IsMerged = true;
                });

                await Task.WhenAll(tasks);

                await model.SaveChangesAsync();
            }
        }

        public List<HistoriesGroup> GetGroupsNotMergedByEvent(int count)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                model.Configuration.AutoDetectChangesEnabled = false;
                model.Configuration.ValidateOnSaveEnabled = false;
                model.Database.CommandTimeout = CONNECTION_TIMEOUT;

                var histories = model.dvs_eventhistory
                        .AsNoTracking()
                        .Where(history => history.IsActive && history.IsMerged == false)
                        .OrderBy(history=>history.CreatedOn)
                        .Take(count)
                        .ToList()
                        .Select(NewEventHistoryFrom)
                        .ToList();

                var groups = histories.GroupBy(rec => rec.EventId)
                    .Select(rec => new HistoriesGroup
                    {
                        EventId = rec.Key,
                        EventHistories = rec.Select(x => x).ToList()
                    });

                var historiesGroups = groups as HistoriesGroup[] ?? groups.ToArray();

                var eventIds = historiesGroups.Select(x => x.EventId);

                var query = model.dvs_event
                    .AsNoTracking()
                    .Where(@event => @event.IsActive && eventIds.Contains(@event.EventId))
                    .Select(x=> new { EventId = x.EventId, XmlText = x.XmlText});

                var events = query.ToList();

                foreach ( var group in historiesGroups)
                {
                    foreach ( var @event in events)
                    {
                        if ( @event.EventId == group.EventId)
                        {
                            group.BaseEventXmlText = @event.XmlText;
                        }
                    }                    
                }

                return historiesGroups.Where(group=>group.BaseEventXmlText!=null).ToList();
            }
        }
        */

        public EventHistory Insert(EventHistory  history)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                var newRecord = NewRepositoryEventHistoryFrom(history);

                model.dvs_eventhistory.Add(newRecord);
                model.SaveChanges();

                history.Id = newRecord.Id;

                return history;
            }
        }

        public async Task BulkInsert(IList<EventHistory> inserting)
        {
            using (var model = new gb_dvsstagingEntities())
            {
                model.Configuration.AutoDetectChangesEnabled = false;
                model.Configuration.ValidateOnSaveEnabled = false;
                model.Database.CommandTimeout = CONNECTION_TIMEOUT;


                // will add by counter events
                for (int i=0; i <= inserting.Count()/ INSERT_COUNT; i++)
                {
                    var subHistories = inserting.Skip(i * INSERT_COUNT).Take(INSERT_COUNT).ToList();

                    var histories = subHistories.Select(NewRepositoryEventHistoryFrom).ToList();

                    model.dvs_eventhistory.AddRange(histories);
                    await model.SaveChangesAsync();
                }
            }
        }

        public static dvs_eventhistory NewRepositoryEventHistoryFrom(EventHistory history)
        {
            return new dvs_eventhistory
            {
                EventId = history.EventId,
                XmlText = history.XmlText,
                CreatedBy = Admin,
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = Admin,
                UpdatedOn = DateTime.UtcNow,
                IsActive = true,
                IsMerged = false
            };
        }

        public static dvs_eventhistory NewRepositoryEventHistoryFrom(Event @event)
        {
            return new dvs_eventhistory
            {
                EventId = @event.EventId,
                // XmlText = @event.XmlTextExample,
                CreatedBy = Admin,
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = Admin,
                UpdatedOn = DateTime.UtcNow,
                IsMerged = false,
                IsActive = true
            };
        }

        public static EventHistory NewEventHistoryFrom(dvs_eventhistory history)
        {
            return new EventHistory
            {
                Id = history.Id,
                EventId = history.EventId,
                XmlText = history.XmlText,
                CreatedOn = history.CreatedOn
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

        public Event GetItemById(long eventId)
        {
            throw new System.NotImplementedException();
        }
    }
}