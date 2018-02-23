using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LSports.SchedulerPushServiceWebAPI.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet]
        public string Test()
        {
            return "dog-nail";
        }
    }
}
