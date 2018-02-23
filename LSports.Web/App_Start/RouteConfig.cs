using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LSports
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { area = "Validation", controller = "Product", action = "Index", id = UrlParameter.Optional },
                null,
                new String[] { "LSports.Areas.Validation.Controllers" }
            ).DataTokens.Add("area", "Validation");


        }
    }
}
