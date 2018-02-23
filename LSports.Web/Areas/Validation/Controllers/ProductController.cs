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
using LSports.Services;
using LSports.Services.Interfaces;
using LSports.Services.ViewModels;

namespace LSports.Areas.Validation.Controllers
{
    //[Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IValidationRuleRepository _validationRuleRepository;
        private readonly IProductTreeRepository _productTreeRepository;


        public ProductController() : this(new ValidationRuleRepository(), new ProductService(), new ProductTreeRepository())
        {
        }

        public ProductController(IValidationRuleRepository validationRuleRepository, IProductService productService, IProductTreeRepository productTreeRepository)
        {
            _validationRuleRepository = validationRuleRepository;
            _productService = productService;
            _productTreeRepository = productTreeRepository;
        }

        // Actions

        public ActionResult Index()
        {
            return View();
        }

        /* Parameters:	show enum EParameter

        /* Operators:
         *  '1', 'Equals', 
            '2', 'Not Equal', 
            '3', 'Greater than', 
            '4', 'Greater than or Equal to', 
            '5', 'Lower than', 
            '6', 'Lower than or Equal to', 
            '7', 'Contains', 
            '8', 'Not Contains', 
            '9', 'Starts with', 
            '10', 'Not starting with', 
            '11', 'End with', 
            '12', 'Not ending with', 
            '13', 'Between', 
            '14', 'Not between', 
            '15', 'Regex', 
            '16', 'Timeframe', '2016-10-05 19:08:47', '', '1', NULL, NULL
         */

        public JsonResult GetOperatorsToParametersRelations()
        {
            Dictionary<string, List<String>> relations = new Dictionary<string, List<string>>();

            var relationsValue = new List<string>();
            relationsValue.AddRange(new string[] {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "15", "17", "18", "19", "20", "21", "22", "23", "24" });
            relations.Add("1", relationsValue);

            var relationsLength = new List<string>();
            relationsLength.AddRange(new string[] { "1", "2", "3", "4", "5", "6", "13", "14", "15"});
            relations.Add("2", relationsLength);

            var relationsNumberOfDistinctChanges = new List<string>();
            relationsNumberOfDistinctChanges.AddRange(new string[] { "1", "2","3","4","5","6","13","14" });
            relations.Add("3", relationsNumberOfDistinctChanges);

            var relationsNumberOfChanges = new List<string>();
            relationsNumberOfChanges.AddRange(new string[] { "1", "2", "3", "4", "5", "6", "13", "14" });
            relations.Add("4", relationsNumberOfChanges);

            var relationsDataType = new List<string>();
            relationsDataType.AddRange(new string[] {"1", "2", "15"});
            relations.Add("5", relationsDataType);

            return Json(relations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetXMLData(int productId)
        {
            // This returned data is a sample. You should get it using some logic
            // This can be an object or an anonymous object like this:

            var model = _productTreeRepository.GetItemByProductId(productId).Tree;

            return Json(model, JsonRequestBehavior.AllowGet);
        }



        // Data working

        public JsonResult List(DateTime from, DateTime to)
        {
            var products = _productService.GetAll();
            List<ProductViewModel> models = new List<ProductViewModel>();

            foreach (var product in products)
            {
                var model = new ProductViewModel
                {
                    Product = product,
                    GraphData = _productService.GetHistoricalData(product.Id, from, to)
                };

                model.NumberOfIssues = model.GraphData.Values.Count > 0 ? model.GraphData.Values.Aggregate((count, x) => count + x) : 0;

                models.Add(model);
            }

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAll()
        {
            return Json(_productService.GetAll(), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProduct(int id)
        {
            return Json(_productService.GetProduct(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LastArrivalMessagesInfo(int? id)
        {
            return Json(_productService.LastArrivalMessagesInfo(id), JsonRequestBehavior.AllowGet);
        }
        /*
        public JsonResult GetHistoricalData(int productId, DateTime from, DateTime to)
        {
            var result = _productService.GetHistoricalData(productId, from, to);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        */
    }
}