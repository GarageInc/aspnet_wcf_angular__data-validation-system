using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using LSports.Scheduler.Jobs.@base;

namespace LSports.Scheduler.Jobs
{

	/// <summary>
	/// This reporter send email each N hours with info about DVS state.
	/// </summary>
    [DisallowConcurrentExecution]
    public class ReporterSheduler : XmlScheduler
    {
        private readonly IScheduler _scheduler = StdSchedulerFactory.GetDefaultScheduler();

        override public void Start()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            _scheduler.Start();

            StartCronWorker<DailyReporter>("1", "DailyReporter");
        }

        public override void Stop()
        {
            var keys = _scheduler.GetTriggerKeys(null);
            _scheduler.UnscheduleJobs(keys.ToList());
            _scheduler.Shutdown();
        }
    }
}