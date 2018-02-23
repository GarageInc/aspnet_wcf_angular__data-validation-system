using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSports.Scheduler;
using LSports.Scheduler.Jobs;
using LSports.Scheduler.Listeners;

namespace LSports.EventsService
{
    class Program
    {
        static void Main(string[] args)
        {
            var portString = ConfigurationManager.AppSettings[SchedulerConfig.EventsServicePortAlias];

            var port = int.Parse(portString);

            var eventsListener = new EventsHttpServer("127.0.0.1", port);
            
            Console.WriteLine("Servers started and binded to: " + port);

            var intervalRequestsToWebApi = new DogNailWebapiSheduler();
            intervalRequestsToWebApi.Start();

            Console.WriteLine("Requests to webapi started");

        }
    }
}
