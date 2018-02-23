using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.Enums
{
    public enum SchedulerTypes
    {
        Downloader = 1,
        EventsCreator = 2,
        EventsValidator = 3,
        Merger = 4,
        ProcessingArrivalMessage = 5,
        SelectionTreeItemCreator = 6,
        ProcessingAllArrivalMessages = 7,
        ParseXmlFile = 8,
        UpdatingArrivalMessages = 9,
		UpdatingLastValidateAt = 10
	}
}
