using System;
using System.Web.Mvc;

namespace LSports.Areas.Validation
{
    public class ValidationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Validation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Validation_default",
                "Validation/{controller}/{action}/{id}",
                new { area="Validation", controller="Settings",action = "Index", id = UrlParameter.Optional },
                new String[] { "LSports.Areas.Validation.Controllers" }
            );
        }
    }
}