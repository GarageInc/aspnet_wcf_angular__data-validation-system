using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LSports.Framework.Models.CustomClasses;

namespace LSports.Scheduler
{
	// Storage for events for one product(1 queue for 1 product)
	public class ProductStorage
    {
        public Dictionary<int, ConcurrentQueue<string>> store = new Dictionary<int, ConcurrentQueue<string>>();

        public void Init(IList<Product> products)
        {
            foreach (var product in products)
            {
                if (store.ContainsKey(product.Id) == false)
                {
                    store.Add(product.Id, new ConcurrentQueue<string>());
                }// pass
            }
        }
    }

	// Queue for all products. It is divided on N queues
    public class ProductsEventsConcurencyQueue
    {
        protected static ProductsEventsConcurencyQueue Instance;

        public static ProductsEventsConcurencyQueue GetInstance()
        {
            return Instance ?? (Instance = new ProductsEventsConcurencyQueue());
        }

        public ProductStorage Storage { get; set; }

        public ProductsEventsConcurencyQueue()
        {
            Storage = new ProductStorage();
        }

        public void Init(IList<Product> products)
        {
            Storage.Init(products);
        }

        public void Add(int productId, string message)
        {
            Storage.store[productId].Enqueue(message);
        }

        public string GetForProduct(int productId)
        {
            var result = "";

            Storage.store[productId].TryDequeue(out result);

            return result;
        }

    }
}