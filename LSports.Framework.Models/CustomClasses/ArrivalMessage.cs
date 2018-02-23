using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.CustomClasses
{
    public class ArrivalMessage
    {
        public int Id { get; set; }
        // public string LastUpdate { get; set; }
        public string XmlMessage { get; set; }
        public int ProductId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public List<string> Events { get; set; }
        public List<string> Sports { get; set; }
        public List<string> Leagues { get; set; }
        public List<string> Locations { get; set; }
        public List<string> Markets { get; set; }
        public List<string> Providers { get; set; }
        public List<string> Bets { get; set; }
        public List<string> Statuses { get; set; }

        public int? EventsCount { get; set; }
        public int? SportsCount { get; set; }
        public int? LeaguesCount { get; set; }
        public int? LocationsCount { get; set; }
        public int? MarketsCount { get; set; }
        public int? ProvidersCount { get; set; }
        public int? BetsCount { get; set; }
        public int? StatusesCount { get; set; }

        public bool IsProcessed { get; set; }
        public string PathToXmlFile { get; set; }

        public virtual Product Product { get; set; }

        public string GroupId { get; set; }

        public string Url { get; set; }

        public void SetFields(bool isDistincts, List<string> eventNames, List<string> sportNames,
            List<string> leagueNames, List<string> locationNames,
            List<string> statusNames, List<string> marketNames,
            List<string> providerNames, List<string> betNames)
        {
            if (isDistincts)
            {
                eventNames = eventNames.Distinct().ToList();
                locationNames = locationNames.Distinct().ToList();
                leagueNames = leagueNames.Distinct().ToList();
                sportNames = sportNames.Distinct().ToList();
                statusNames = statusNames.Distinct().ToList();
                marketNames = marketNames.Distinct().ToList();
                providerNames = providerNames.Distinct().ToList();
                betNames = betNames.Distinct().ToList();
            }

            this.Events = eventNames;
            this.Locations =locationNames;
            this.Leagues = leagueNames;
            this.Sports = sportNames;
            this.Statuses = statusNames;
            this.Markets = marketNames;
            this.Providers = providerNames;
            this.Bets = betNames;

            this.EventsCount = eventNames.Count;
            this.LocationsCount = locationNames.Count;
            this.LeaguesCount = leagueNames.Count;
            this.SportsCount = sportNames.Count;
            this.StatusesCount = statusNames.Count;
            this.MarketsCount = marketNames.Count;
            this.ProvidersCount = providerNames.Count;
            this.BetsCount = betNames.Count;

            this.UpdatedOn = DateTime.UtcNow;
        }
    }
}
