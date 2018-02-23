using log4net;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.Scheduler.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace LSports.Areas.Validation.Controllers
{
    public class SchedulersHistoryController : Controller
    {
        protected ISchedulerHistoryRepository _schedulersHistoryRepo;

        public SchedulersHistoryController() : this(new SchedulerHistoryRepository())
        {
        }

        public SchedulersHistoryController(ISchedulerHistoryRepository schedulersHistoryRepo)
        {
            _schedulersHistoryRepo = schedulersHistoryRepo;
        }


        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetIP()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // `Dns.Resolve()` method is deprecated.
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            int intAddress = BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0);

            return Json(new { host = Request.ServerVariables["LOCAL_ADDR"], ip = new IPAddress(BitConverter.GetBytes(intAddress)).ToString() }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetHistoricalDataForProduct(int productId, DateTime from, DateTime to)
        {
            var result = _schedulersHistoryRepo.GetHistoryByProductId(productId, from, to);

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }


        public JsonResult GetHistoricalDataEvents(DateTime from, DateTime to)
        {
            var result = _schedulersHistoryRepo.GetHistoryOfValidation(from, to);

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
    }
}