using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using LSports.Framework.Models.CustomClasses;
using System.Collections.Concurrent;

namespace LSports.Scheduler
{
	/// <summary>
	/// Glogbal events store with/without locker on Add-actions
	/// </summary>
    public class EventsStoreAndLocker
    {
        protected int MergeCounter = 80;

        // protected object locker = new Object();

        protected EventsStore store = new EventsStore();

        /// <summary>
        /// Adding not repeated in one collection. Not Concurrency
        /// </summary>
        /// <param name="oldEvents"></param>
        /// <param name="newEvents"></param>
        public void AddNotRepeated(List<Event> oldEvents, List<Event> newEvents)
        {
            if (oldEvents.Count > 0 || newEvents.Count > 0)
            {
                {
                    if (oldEvents.Any())
                    {
                        //store.InitLocker(oldEvents);

                        DivideByMergeCounterAndWait(newEvents);
                    }//

                    if (newEvents.Any())
                    {
                        //store.InitLocker(newEvents);

                        DivideByMergeCounterAndWait(newEvents);
                    }//
                }
            }
        }

		/// <summary>
		/// Helper-function for dividing events for parallel processing. All events are processed by groups in tasks.
		/// Is is used for PULL-messages!! Because on big xml contains always unique events
		/// </summary>
		/// <param name="events"></param>
        public void DivideByMergeCounterAndWait(List<Event> events)
        {
            var tasks = new List<Task>();
            for (var i = 0; i <= events.Count / MergeCounter; i++)
            {
                var subEvents = events.Skip(i * MergeCounter).Take(MergeCounter).ToList();

                if (subEvents.Any())
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        foreach (var t in subEvents)
                        {
                            store.Add(t);
                        }
                    }));
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

		// Another way for processing tasks for PUSH-messages
		// We group them by id, because there are a lot identical events
        public void DivideByEventIdAndWait(List<Event> events )
        {
            var groups = events.GroupBy(x => x.EventId);

            var tasks = new List<Task>();
            foreach (var @group in groups)
            {
                var groupEvents = @group.OrderBy(x => x.LastUpdate).ToList();

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    foreach (var t in groupEvents)
                    {
                        store.Add(t);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }
        /// <summary>
        /// Adding repeated in one collection. Concurrency.
        /// </summary>
        /// <param name="oldEvents"></param>
        /// <param name="newEvents"></param>
        public void AddRepeated(List<Event> oldEvents, List<Event> newEvents)
        {
            if (oldEvents.Count > 0 || newEvents.Count > 0)
            {
                {
                    if (oldEvents.Any())
                    {
                        //store.InitLocker(oldEvents);
                        DivideByEventIdAndWait(oldEvents);
                    }//

                    if (newEvents.Any())
                    {
                        //store.InitLocker(newEvents);

                        DivideByEventIdAndWait(newEvents);
                    }//
                }
            }
        }
        
		/// <summary>
		/// Getting list if all saved in memory events
		/// </summary>
		/// <returns></returns>
        public List<Event> GetEvents()
        {
            return store.GetEvents();
        }


		/// <summary>
		/// Clearing from memory
		/// </summary>
        public void Clear()
        {
            store.Clear();
        }
    }
}