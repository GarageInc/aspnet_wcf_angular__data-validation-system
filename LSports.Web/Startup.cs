using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LSports.Startup))]
namespace LSports
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //XMLSheduler.Start();

            //GlobalConfiguration.Configuration
            //.UseSqlServerStorage(@"Server=MACMINIPC\SQLEXPRESS; Database=Hangfire; Integrated Security=SSPI;");

            //app.UseHangfireDashboard("/jobs");

            //app.UseHangfireServer();

            //RecurringJob.AddOrUpdate(() => new XmlDownloadWorker().Run(), 
            //    Cron.MinuteInterval(15));
            //RecurringJob.AddOrUpdate(() => new XmlValidateWorker().Run(),
            //    Cron.MinuteInterval(15));
            //RecurringJob.AddOrUpdate(() => new XmlMergeWorker().Run(),
            //    Cron.MinuteInterval(15));
            //Code to setup getting emails
        }
    }
}
