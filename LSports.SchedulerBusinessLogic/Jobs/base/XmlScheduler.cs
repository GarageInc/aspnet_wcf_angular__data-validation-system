using System;
using System.Linq;
using Quartz;
using Quartz.Impl;

namespace LSports.Scheduler.Jobs.@base
{
	/// <summary>
	/// Base class for all schedulers
	/// Start of cron-tasks.
	/// </summary>
    public class XmlScheduler
    {

        private readonly IScheduler _scheduler = StdSchedulerFactory.GetDefaultScheduler();

        public virtual void Start()
        {
            throw new NotImplementedException();;
        }

        public void StartWorker<T>(TimeSpan interval_milliseconds, string identity, string group) where T : IJob
        {
            IJobDetail job = JobBuilder.Create<T>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(identity, group)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(interval_milliseconds)// interval
                    .RepeatForever())
                .Build();

            _scheduler.ScheduleJob(job, trigger);
        }


		/// <summary>
		/// Cron-worket, which is start on selected cron-time.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="identity"></param>
		/// <param name="group"></param>
        public void StartCronWorker<T>(string identity, string group) where T : IJob
        {
            IJobDetail job = JobBuilder.Create<T>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                                .WithIdentity(identity, group)
                                .WithCronSchedule("0 0 21 * * ?")// 21:00 each evening
                                //.WithCronSchedule("0 0/1 * * * ?")
                                .ForJob(job)
                                .Build();

            _scheduler.ScheduleJob(job, trigger);
        }

        public virtual void Stop()
        {
            throw new NotImplementedException();
        }
    }
}