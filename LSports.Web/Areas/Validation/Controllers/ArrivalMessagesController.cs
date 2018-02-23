using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.Services;
using LSports.Services.Interfaces;
using Newtonsoft.Json;

namespace LSports.Areas.Validation.Controllers
{
    public class ArrivalMessagesController : Controller
    {
        private readonly IArrivalMessageService _arrivalMessageService;
        private readonly IArrivalMessageRepository _arrivalMessageRepository;
        private readonly IValidationResultRepository _validationResultRepository;

        public ArrivalMessagesController() : this(new ValidationResultRepository(), new ArrivalMessageService(), new ArrivalMessageRepository())
        {
        }

        public ArrivalMessagesController(IValidationResultRepository validationResultRepository, IArrivalMessageService arrivalMessageService, IArrivalMessageRepository arrivalMessageRepo)
        {
            _validationResultRepository = validationResultRepository;
            _arrivalMessageService = arrivalMessageService;
            _arrivalMessageRepository = arrivalMessageRepo;
        }
        
        public JsonResult GetArrivalMessages(int productId, FilterObject filter, int offset, int count)
        {
            var result = _arrivalMessageService.GetArrivalMessages(productId, filter, offset, count);

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public JsonResult GetDataForChart(int productId, FilterObject filter)
        {
            var result = _arrivalMessageRepository.GetChartDataForProduct(productId, filter);

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        

        public JsonResult GetCountOfArrivalMessages(int productId, FilterObject filter)
        {
            var result = _arrivalMessageService.GetCountOfArrivalMessages(productId, filter);

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public ActionResult ForProduct(int id)
        {
            ViewBag.ProductId = id;
            return View();
        }
    }
}