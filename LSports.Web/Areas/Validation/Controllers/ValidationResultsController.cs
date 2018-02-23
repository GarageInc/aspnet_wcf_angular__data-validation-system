using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.Services;
using LSports.Services.Interfaces;

namespace LSports.Areas.Validation.Controllers
{
    public class ValidationResultsController : Controller
    {
        private readonly IValidationSettingService _validationSettingService;
        private readonly IValidationResultRepository _validationResultRepository;

        public ValidationResultsController() : this(new ValidationSettingService(), new ValidationResultRepository())
        {
        }

        public ValidationResultsController(IValidationSettingService validationSettingService, IValidationResultRepository validationResultRepository)
        {
            _validationSettingService = validationSettingService;
            _validationResultRepository = validationResultRepository;
        }

        public JsonResult GetValidationSettingsForProduct(int? id, FilterObject filter)
        {
            var result = _validationSettingService.GetValidationSettingsForProduct(id, filter);

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        public JsonResult GetValidationResultsBySettingId(int settingId, int productId, int offset, int count, FilterObject filter, bool loadInactive)
        {
            var result = _validationResultRepository.GetResultsBySettingId(settingId, productId, offset, count, filter, loadInactive);

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        
        public ActionResult GetDropdownValues(int? productId, int filterType, FilterValue selectedFilterValue)
        {
            return Json(_validationResultRepository.GetDropDownValues(productId, filterType, selectedFilterValue), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetXmlForValidationResult(int eventId, bool isHistoricalAttributes, int productId)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            byte[] data = null;
            var xml = "<xml server_error/>";
            try
            {

                string sURL = $"{ConfigurationManager.AppSettings["EventServicePath"]}/?eventId={eventId}&productId={productId}";

                WebRequest wrGETURL = WebRequest.Create(sURL);

                var xmlText = "";

                MemoryStream ms = new MemoryStream();
                wrGETURL.GetResponse().GetResponseStream().CopyTo(ms);

                data = ms.ToArray();

                var fileData = new StringBuilder();

                using (var msi = new MemoryStream(data))
                {
                    using (var zipStream = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        var tempBytes = new byte[4096];
                        int i;
                        while ((i = zipStream.Read(tempBytes, 0, tempBytes.Length)) != 0)
                            fileData.Append(Encoding.UTF8.GetString(tempBytes, 0, i));
                    }
                }

                XDocument xmlDoc = XDocument.Parse(fileData.ToString());

                if (!isHistoricalAttributes)
                {
                    FilterXmlNodesByHistory(xmlDoc, "DvsHistoryValues");
                    FilterXmlNodesByHistory(xmlDoc, "DvsHistoryAttributes");
                }

                xml = xmlDoc.ToString();

            }
            catch (Exception e)
            {
                xml = e.Message;

                if (data != null)
                {

                    xml += "\n Received: '" + Encoding.UTF8.GetString(data) + "'";
                }

                var failedResult = Json(xml, JsonRequestBehavior.AllowGet);
                failedResult.MaxJsonLength = int.MaxValue;

                return failedResult;
            }

            var jsonResult = Json(xml, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;

        }

        public static void FilterXmlNodesByHistory(XDocument doc, string nodeName)
        {
            var q = from node in doc.Descendants(nodeName)
                    select node;

            q.ToList().ForEach(x => x.Remove());
        }

        public ActionResult ForProduct(string id)
        {
            ViewBag.ProductId = id;
            return View();
        }

        public ActionResult GetValidationResult(int id)
        {
            var jsonResult = Json(_validationResultRepository.Get(id), JsonRequestBehavior.AllowGet);
            return jsonResult;
        }


        public ActionResult DisableForSettingEvent(int productId, int settingId, int eventId)
        {
            var jsonResult = Json(_validationResultRepository.DisableForSettingEvent(productId, settingId, eventId), JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
    }
}