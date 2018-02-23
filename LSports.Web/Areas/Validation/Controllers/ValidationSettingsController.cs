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
    public class ValidationSettingsController : Controller
    {
        private readonly IValidationSettingRepository _validationSettingRepository;
        private readonly IValidationRuleRepository _validationRuleRepository;
        private readonly IValidationResultRepository _validationResultRepository;



        public ValidationSettingsController() : this(new ValidationSettingRepository(), new ValidationRuleRepository(), new ValidationResultRepository())
        {

        }

        public ValidationSettingsController(IValidationSettingRepository validationSettingRepository, IValidationRuleRepository validationRuleRepository, IValidationResultRepository validationResultRepository)
        {
            _validationSettingRepository = validationSettingRepository;
            _validationRuleRepository = validationRuleRepository;
            _validationResultRepository = validationResultRepository;
        }

        // Actions

        public ActionResult Index(int? id)
        {
            ViewBag.SettingId = id ?? 0;
            return View();
        }

        // Data working

        public JsonResult List()
        {
            return Json(_validationSettingRepository.GetList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Create(ValidationSetting validationSetting)
        {   
            var validationSettingId = _validationSettingRepository.Insert(validationSetting);

            if (validationSetting.ValidationRules != null)
            {
                foreach (var rule in validationSetting.ValidationRules)
                {
                    rule.ValidationSettingId = validationSettingId;
                    _validationRuleRepository.Insert(rule);
                }
            }

            return Json(validationSetting.Id, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Update(ValidationSetting validationSetting)
        {
            var validationSettingId = _validationSettingRepository.Update(validationSetting);

            if(validationSetting.ValidationRules != null)
            {
                foreach (var rule in validationSetting.ValidationRules)
                {
                    rule.ValidationSettingId = validationSettingId;

                    if (rule.Id < 0)
                    {
                        _validationRuleRepository.Insert(rule);
                    }
                    else
                    {
                        _validationRuleRepository.Update(rule);
                    }
                }
            }

            _validationResultRepository.SetDisabledBySetting(validationSetting.Id);

            return Json(validationSetting.Id, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            _validationSettingRepository.Delete(id);

            return Json(0, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetValidationOperators()
        {
            return Json(_validationSettingRepository.GetValidationOperators(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetValidationParameters()
        {
            return Json(_validationSettingRepository.GetValidationParameters(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetValidationRules(int validationSetttingId)
        {
            return Json(_validationSettingRepository.GetValidationRules(validationSetttingId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetValidationDataTypes()
        {
            return Json(_validationSettingRepository.GetValidationDataTypes(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Partial()
        {
            return View();
        }
    }
}