using System.Collections.Generic;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories.Interfaces
{
    public interface IEventRepository
    {
        IList<Event> GetList();
        IList<Event> GetListByOffsetAndCountForProduct(int productId, int offset, int count);
        List<long> GetListEventsIds();

        List<Event> GetEventsByEventIds(List<long> eventIds, int productId);

        int GetCountForProduct(int productId);

        Event Insert(Event @event);


        void BulkInsert(IList<Event> events);
        
        void BulkInsertByCreatingHistories(IList<Event> events);

        Event Update(Event department);


        void Delete(int id);

        bool IsEventNameUnique(string name, int id);

        Event GetItemByEventId(long eventId);
    }
}
