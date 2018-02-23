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
	/// <summary>
	/// Worker for pull-service. Three steps.
	/// 1. Usual downloader.
	/// 2. Reset downloader. It reset using for downloading by selected timestamp(not last downloaded). It can be 1 hour, one day, as it was set in web.config ResetDownloaderIntervalAlias
	/// 2. Processing downloaded messages(pull-messages)
	/// </summary>
	[DisallowConcurrentExecution]
    public class XmlPullSheduler: XmlScheduler
    {
        private readonly IScheduler _scheduler = StdSchedulerFactory.GetDefaultScheduler();

        override public void Start()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            _scheduler.Start();

            StartWorker<XmlDownloadWorker>(SchedulerConfig.DownloadingTimeoutInMs, "1", "XmlDownloadWorker");

			// We must parse downloading interval from config
			var resetDownloadingTimeout = 0;
            var parsed = int.TryParse(ConfigurationManager.AppSettings[SchedulerConfig.ResetDownloaderIntervalAlias], out resetDownloadingTimeout);
            if (parsed)
            {
                var timespanSeconds = new TimeSpan(0, 0, 0, resetDownloadingTimeout);
                StartWorker<XmlResetDownloadWorker>(timespanSeconds, "2", "XmlResetDownloadWorker");
            }

			// Start pull-worker(for processing downloaded xmls)
			StartWorker<XmlPullCommonWorker>(SchedulerConfig.PullCommonWorkerTimeoutInMs, "3", "XmlPullCommonWorker");
        }

        public override void Stop()
        {
            var keys = _scheduler.GetTriggerKeys(null);
            _scheduler.UnscheduleJobs(keys.ToList());
            _scheduler.Shutdown();
        }
    }
}