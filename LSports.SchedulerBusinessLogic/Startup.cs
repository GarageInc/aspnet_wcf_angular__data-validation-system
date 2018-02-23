using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LSports.Scheduler.Startup))]
namespace LSports.Scheduler
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
