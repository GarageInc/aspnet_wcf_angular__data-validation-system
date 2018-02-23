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
using LSports.Framework.DataAccess.Repositories.Interfaces;

namespace LSports.Areas.Validation.Controllers
{
    //[Authorize]
    public class ValidationRulesController : Controller
    {
        protected readonly IValidationRuleRepository _validationRuleRepository;


        public ValidationRulesController() : this(new ValidationRuleRepository())
        {

        }

        public ValidationRulesController(IValidationRuleRepository validationRuleRepository)
        {
            _validationRuleRepository = validationRuleRepository;
        }
        

        [HttpPost]
        public JsonResult Delete(int id)
        {
            _validationRuleRepository.Delete(id);

            return Json(id, JsonRequestBehavior.AllowGet);
        }        
        
    }
}