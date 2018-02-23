using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.Framework.Models.CustomClasses;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using LSports.Framework.DataAccess.Repositories;
using LSports.Framework.DataAccess.Repositories.Interfaces;

namespace LSports.Areas.Validation.Controllers
{
    //[Authorize]
    public class ValidationPrioritiesController : Controller
    {
        protected readonly IPriorityRepository _priorityRepository;


        public ValidationPrioritiesController() : this(new PriorityRepository())
        {

        }

        public ValidationPrioritiesController(IPriorityRepository priorityRepository)
        {
            _priorityRepository = priorityRepository;
        }

        public JsonResult List()
        {
            return Json(_priorityRepository.List(), JsonRequestBehavior.AllowGet);
        }        
    }
}