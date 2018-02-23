
namespace LSports.Scheduler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Xml;
    using LSports.Framework.Models.CustomClasses;
    using LSports.Scheduler.Services;
    using System.Collections.Concurrent;

	// Cocncurrent event's store for received events.
    public class EventsStore
    {
        public ConcurrentDictionary<long, Event> store = new ConcurrentDictionary<long, Event>();

		// Events merger(on Add-action)
        protected XmlMerger merger = new XmlMerger();

		// Lockers for concurrent execution for each type of event
        public Dictionary<string, Object> EventsUniqueLockers = new Dictionary<string, object>();

		// Common locker 
        public Object CommonLocker = new Object();

		// Initialization of lockers
        public void InitLocker(List<Event> events)
        {
            lock (CommonLocker)
            {
                foreach (var @event in events)
                {
                    var key = @event.GetLockerKey();

                    if (!EventsUniqueLockers.ContainsKey(key))
                    {
                        EventsUniqueLockers.Add(key, new Object());
                    }
                }
            }
        }

		// Adding new event to store
        public void Add(Event newEvent)
        {
            var newEventIsNotNull = newEvent.XmlTextExample.Root != null;

            if (newEventIsNotNull)
            {
				// Using concurrent queue
                store.AddOrUpdate(newEvent.EventId, newEvent, (eventId, oldEvent) =>
                {
                    //lock (EventsUniqueLockers[newEvent.GetLockerKey()])
                    {
						// if it is new event(comparing) we merge this event with old
                        if (DateTime.Compare(newEvent.LastUpdate, oldEvent.LastUpdate) > 0 && newEvent.XmlTextExample != null && oldEvent.XmlTextExample != null)
                        {
                            merger.MergeWithOldChanges(oldEvent.XmlTextExample.Elements(),
                                newEvent.XmlTextExample.Elements(), oldEvent.XmlTextExample.Root,
                                oldEvent.XmlTextExample.Root, "");

                            newEvent.XmlTextExample = oldEvent.XmlTextExample;
                            oldEvent.XmlTextExample = null;// clearing from memory

                            newEvent.LastUpdate = oldEvent.LastUpdate;
                            newEvent.CreatedOn = oldEvent.CreatedOn;
                            newEvent.UpdatedOn = DateTime.UtcNow;

                            return newEvent;
                        }
                        else
                        {
                            return oldEvent;
                        }
                    }
                });
            }
        }

		// saving each event
        public void SaveAll()
        {
            foreach (var keyValue in store)
            {
                keyValue.Value.Save();
            }
        }

        public void Clear()
        {
            this.store.Clear();
        }

        public List<Event> GetEvents()
        {
            return store.Values.ToList();
        }
    }
}