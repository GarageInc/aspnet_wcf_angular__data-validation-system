using System.Collections.Generic;
using System.Threading.Tasks;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories.Interfaces
{
    public interface IEventHistoryRepository
    {
        IList<EventHistory> GetList();
        
        // List<HistoriesGroup> GetGroupsNotMergedByEvent(int count);

        //void SaveHistoriesGroups(List<HistoriesGroup> group);

        int GetCount();
        int GetNotMergedCount();

        EventHistory Insert(EventHistory @event);
        
        Task BulkInsert(IList<EventHistory> events);
    }
}
