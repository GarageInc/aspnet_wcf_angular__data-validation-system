using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using LSports.Scheduler;
using LSports.Scheduler.Jobs;

namespace LSports.SchedulerPushServiceWebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {

		// Entry ppint. Here all services start.
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

			// Checking. Is receiving of push messages was enabled?
            var isPushEnabled = bool.Parse(ConfigurationManager.AppSettings[SchedulerConfig.IsPushEnabledAlias]);

            if (isPushEnabled)
            {
                // Initialization global queue with push-services(products).
                var productsIds = new List<int> { 2, 3, 4, 5 };
                foreach (var product in productsIds)
                {
                    ProductsEventsConcurencyQueue.GetInstance().Storage.store.Add(product, new ConcurrentQueue<string>());
                }

				// Push-scheduler start. It process all push messages.
                var pushScheduler = new LSports.Scheduler.Jobs.XmlPushSheduler();
                pushScheduler.Start();
            }

			// Is pull-service enabling? Pull service always download events from Lsports-server.
            var isPullEnabled = bool.Parse(ConfigurationManager.AppSettings[SchedulerConfig.IsPullEnabledAlias]);

            if (isPullEnabled)
            {
                var pullScheduler = new LSports.Scheduler.Jobs.XmlPullSheduler();
                pullScheduler.Start();
            }

			// This scheduler always each N hours sent report about DVS work. This email send in email, which was set in web.config.
            var reporter = new ReporterSheduler();
            reporter.Start();
        }
    }
}
