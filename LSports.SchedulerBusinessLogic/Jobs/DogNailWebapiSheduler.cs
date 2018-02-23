using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using LSports.Scheduler.Jobs.@base;

namespace LSports.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class DogNailWebapiSheduler : XmlScheduler
    {
        private readonly IScheduler _scheduler = StdSchedulerFactory.GetDefaultScheduler();
		/// <summary>
		/// Each N seconds we send from EventsService simple http-request for IIS. And IIS does not kill our applications due to inactivity
		/// </summary>
		override public void Start()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            _scheduler.Start();

			// parsing enterval in seconds from web.config param.
            var resetDownloadingTimeout = 0;
            var parsed = int.TryParse(ConfigurationManager.AppSettings[SchedulerConfig.EventsServiceWebApiRequestsIntervalAlias], out resetDownloadingTimeout);
            if (parsed)
            {
                var timespanSeconds = new TimeSpan(0, 0, 0, resetDownloadingTimeout);
                StartWorker<DogNailWebapi>(timespanSeconds, "1", "DogNailWebapi");
            }
        }

        public override void Stop()
        {
            var keys = _scheduler.GetTriggerKeys(null);
            _scheduler.UnscheduleJobs(keys.ToList());
            _scheduler.Shutdown();
        }
    }
}