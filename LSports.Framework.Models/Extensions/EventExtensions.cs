using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSports.Framework.Models.CustomClasses;

namespace LSports.Framework.Models.Extensions
{
    public static class EventExtensions
    {
        public static string BuildEventName(this Event @event)
        {
            if (string.IsNullOrEmpty(@event.HomeTeamName) || @event.HomeTeamName == "Null")
            {
                if (string.IsNullOrEmpty(@event.AwayTeamName) || @event.AwayTeamName == "Null")
                {
                    return "Null vs Null";
                }
                return @event.AwayTeamName;
            }

            if (string.IsNullOrEmpty(@event.AwayTeamName) || @event.AwayTeamName == "Null")
            {
                return @event.HomeTeamName;
            }

            return @event.HomeTeamName + " vs " + @event.AwayTeamName;
        }
    }
}
