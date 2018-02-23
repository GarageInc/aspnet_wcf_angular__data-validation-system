using System;
using System.Collections.Generic;
using System.Linq;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.Framework.Models.Extensions;
using System.Text;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class SelectionTreeItemRepository : ISelectionTreeItemRepository
    {
        protected const int CONNECTION_TIMEOUT = 300;
        protected Object thisLock = new Object();

        public IList<SelectionTreeItem> GetListByTypeId(int typeId)
        {
            using (var entities = new gb_dvsstagingEntities())
            {
                var q = entities.dvs_selectiontreeitem.Where(rec => rec.IsActive && rec.TypeId == typeId).Select(rec => new SelectionTreeItem()
                {
                    Id = rec.Id,
                    Name = rec.Name,
                    TypeId = rec.TypeId,
                    ExternalId = rec.ExternalId,
                    ParentId = rec.ParentId
                }).OrderBy(rec => rec.Name);

                return q.ToList();
            }
        }

        public void BulkInsert(IList<Event> events, int? productId) {

            // lock (this.thisLock)
            {
                using (var entities = new gb_dvsstagingEntities())
                {
                    // entities.Configuration.AutoDetectChangesEnabled = false;
                    entities.Configuration.ValidateOnSaveEnabled = false;
                    entities.Database.CommandTimeout = 300;

                    bool isChanged = false;

                    var eventsIds = events.Select(x => x.EventId).ToArray();

                    var allByEvents = entities.dvs_selectiontreeitem.Where(rec => rec.IsActive
                                                                        && eventsIds.Contains((long)rec.ParentId) 
                                                                        && rec.ProductId == productId).ToList();
                    
                    foreach (var @event in events)
                    {
                        isChanged = false;

                        var items = allByEvents.Where(rec => rec.ParentId == @event.EventId).ToList();

                        var market = items.FirstOrDefault(rec => rec.TypeId == 5 && rec.ExternalId == @event.MarketId);

                        if (@event.MarketNames != null && @event.MarketNames.Count > 0)
                        {
                            if (market == null)
                            {
                                market = NewRepositoryTreeItemBy(productId, 5, @event.EventId, @event.MarketId, @event.MarketNames);
                                // market = NewRepositoryTreeItemBy(productId, 5, eventName.Id, @event.MarketId, @event.MarketNames);

                                entities.dvs_selectiontreeitem.Add(market);
                                isChanged = true;
                            }
                            else
                            {
                                var originValues = market.Name.Split(',').ToList();
                                var newValues = @event.MarketNames;
                                market.Name = string.Join(",", originValues.Union(newValues).Distinct().ToList());
                                isChanged = true;
                            }
                        }// pass

                        var provider = items.FirstOrDefault(rec => rec.TypeId == 6 && rec.ExternalId == @event.ProviderId); 

                        if (@event.ProviderNames != null && @event.ProviderNames.Count > 0)
                        {
                            if (provider == null)
                            {
                                //provider = NewRepositoryTreeItemBy(productId, 6, eventName.Id, @event.ProviderId, @event.ProviderNames);
                                provider = NewRepositoryTreeItemBy(productId, 6, @event.EventId, @event.ProviderId, @event.ProviderNames);

                                entities.dvs_selectiontreeitem.Add(provider);
                                isChanged = true;
                            }
                            else
                            {
                                var originValues = provider.Name.Split(',').ToList();
                                var newValues = @event.ProviderNames;
                                provider.Name = string.Join(",", originValues.Union(newValues).Distinct().ToList());
                                isChanged = true;
                            }
                        }// pass

                        var status = items.FirstOrDefault(rec => rec.TypeId == 7 && rec.ExternalId == @event.EventId); ;

                        if (status == null)
                        {
                            // status = NewRepositoryTreeItemBy(productId, 7, eventName.Id, @event.EventId, new List<string> { @event.Status });
                            status = NewRepositoryTreeItemBy(productId, 7, @event.EventId, @event.EventId, new List<string> { @event.Status });

                            entities.dvs_selectiontreeitem.Add(status);
                            isChanged = true;
                        }// pass

                        if (isChanged)
                        {
                            entities.SaveChanges();
                        }
                    }
                }
            }
        }


        public static dvs_selectiontreeitem NewRepositoryTreeItemBy(int? productId, int typeId, long? parentId, long? externalId, IList<string> names)
        {
            return new dvs_selectiontreeitem
            {
                ProductId = productId,
                TypeId = typeId,
                Name = string.Join(",", names),
                ParentId = parentId,
                ExternalId = externalId,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "Admin",
            };
        }

        public void Insert(Event @event)
        {
            throw new NotImplementedException("Insert in SelectionTreeItemRepository");
        }
    }
}