using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LSports.Framework.Models.CustomClasses;

namespace LSports.Scheduler
{
    public class MessagesStore
    {
		// List of all current arrival messages(for pull-service it means list of big xml-files with a lot of events)
        public IList<ArrivalMessage> Messages { get; set; }

        public object locker = new Object();

		/// <summary>
		/// Checking by group(SportId)
		/// </summary>
		/// <param name="groupId"></param>
		/// <returns></returns>
        public bool IsContainsNotProcessedForGroup(string groupId)
        {
            return Messages.Any(x => x.GroupId == groupId && !x.IsProcessed);
        }

		// setting as processed selected message
        public void SetAsProcessed(int messageId)
        {
            for (var i = 0; i < Messages.Count; i++)
            {
                if (Messages[i].Id == messageId)
                {
                    Messages[i].IsProcessed = true;
                }
            }
        }

		/// <summary>
		/// Getting arrival messages by product id
		/// </summary>
		/// <param name="productId"></param>
		/// <returns></returns>
        public List<ArrivalMessage> GetByProductId(int productId)
        {
            return Messages.Where(x => x.ProductId == productId).ToList();
        }
    }
}