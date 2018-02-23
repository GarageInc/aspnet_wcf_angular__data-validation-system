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
	/// Start processing processing push-messages
	/// </summary>
    [DisallowConcurrentExecution]
    public class XmlPushSheduler : XmlScheduler
    {
        private readonly IScheduler _scheduler = StdSchedulerFactory.GetDefaultScheduler();

        public override void Start()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            _scheduler.Start();
            
            StartWorker<XmlPushCommonWorker>(SchedulerConfig.PushWorkerTimeoutInMs, "2", "push_xmlGroup_2");
        }

        public override void Stop()
        {
            var keys = _scheduler.GetTriggerKeys(null);
            _scheduler.UnscheduleJobs(keys.ToList());
            _scheduler.Shutdown();
        }
    }
}