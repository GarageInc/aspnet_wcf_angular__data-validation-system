using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using LSports.Scheduler;

namespace LSports.SchedulerPushServiceWebAPI.Controllers
{
	// Entry points for all push-products.
	// Each message is saved in product's queue
    public class HomeController : ApiController
    {
        [HttpPost]
        public void Product2()
        {
            string xmlMessage = Request.Content.ReadAsStringAsync().Result;

            if (xmlMessage.Contains("<Event"))
            {
                xmlMessage = xmlMessage.Substring(5, xmlMessage.Length - 5);// removed 'data='

                var productId = 2;
                ProductsEventsConcurencyQueue.GetInstance().Storage.store[productId].Enqueue(xmlMessage);
                // 5 is ProductId
            }
        }

        [HttpPost]
        public void Product3()
        {
            string xmlMessage = Request.Content.ReadAsStringAsync().Result;

            if (xmlMessage.Contains("<Event"))
            {
                var productId = 3;
                ProductsEventsConcurencyQueue.GetInstance().Storage.store[productId].Enqueue(xmlMessage);
                // 5 is ProductId
            }
        }

        [HttpPost]
        public void Product4()
        {
            string xmlMessage = Request.Content.ReadAsStringAsync().Result;

            if (xmlMessage.Contains("<Event"))
            {
                var productId = 4;
                ProductsEventsConcurencyQueue.GetInstance().Storage.store[productId].Enqueue(xmlMessage);
                // 5 is ProductId
            }
        }

        [HttpPost]
        public void Product5()
        {
            string xmlMessage = Request.Content.ReadAsStringAsync().Result;

            if (xmlMessage.Contains("<Event"))
            {
                xmlMessage = xmlMessage.Substring(5, xmlMessage.Length - 5);// removed 'data='

                var productId = 5;
                ProductsEventsConcurencyQueue.GetInstance().Storage.store[productId].Enqueue(xmlMessage);
                // 5 is ProductId
            }
        }
    }
}
