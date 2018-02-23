using log4net;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.Services;
using LSports.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LSports.Scheduler;
using LSports.Scheduler.Jobs;
using LSports.Scheduler.Jobs.@base;
using LSports.Scheduler.Services;
using LSports.Services.LoggerServices;

namespace LSports.Areas.Validation.Controllers
{

	/// <summary>
	/// Only for testing API
	/// </summary>
    public class DvsController : Controller
    {
        protected IArrivalMessageService _arrivalMessagesService;

        protected IProductRepository _productRepository;

        protected ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DvsController() : this (new ProductRepository(), new ArrivalMessageService())
        {
        }

        public DvsController( IProductRepository productRepo, IArrivalMessageService arrivalMessageService)
        {
            this._productRepository = productRepo;

            this._arrivalMessagesService = arrivalMessageService;
        }

        public ActionResult ThrowError()
        {
            Logger.GetInstance().Info("Info message");
            Logger.GetInstance().Debug("Debug message");
            Logger.GetInstance().Error("Error message");

            throw new Exception("Custom test exeption");
        }

        // GET: Validation/Dvs
        public ActionResult Index()
        {
            return View();
        }

        public string Upload()
        {
            try
            {
                int productId = 2;

                var product = _productRepository.GetProduct(productId);

                string fileSavingPath = XMLDownloader.GetFileName(productId);

                byte[] bytes = new byte[SchedulerConfig.MaxBytesCount];

//                var file = Request.Files["file"];
                using (System.IO.FileStream fs = System.IO.File.Create(fileSavingPath))
                {
                    int bytesRead;
                    while ((bytesRead = Request.InputStream.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        fs.Write(bytes, 0, bytesRead);
                    }
                }

                _arrivalMessagesService.CreateArrivalMessage(product.Id, "", fileSavingPath, "");

                var message = "success";
                log.Info(message);

                return message;
            } catch(Exception e)
            {
                var message = String.Format("{0}; {1}", e.Message, e.StackTrace);

                log.Error(message);
                return "error";
            }
        }
    }
}