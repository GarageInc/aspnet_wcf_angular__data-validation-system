using System;
using System.Collections.Generic;
using System.Linq;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        protected gb_dvsstagingEntities Entities = new gb_dvsstagingEntities();
        

        public IList<Product> GetListByType(bool IsPushService)
        {
            using (var entities = new gb_dvsstagingEntities())
            {
                var q = entities.dvs_product
                            .Where(rec => rec.IsActive 
                                && rec.IsPushService == IsPushService)
                            .Select(rec => new Product()
                {
                    Id = rec.Id,
                    Name = rec.Name,
                    GatewayAPI = rec.GatewayAPI,
                    IsPushService = rec.IsPushService
                })
                .OrderBy(rec => rec.Id);

                return q.ToList();
            }
        }

        public IList<Product> GetList()
        {
            using (var entities = new gb_dvsstagingEntities())
            {
                var q = entities.dvs_product.Where(rec => rec.IsActive).Select(rec => new Product()
                {
                    Id = rec.Id,
                    Name = rec.Name,
                    GatewayAPI = rec.GatewayAPI

                }).OrderBy(rec => rec.Id);

                return q.ToList();
            }
        }

        public Product GetProduct(int id) {

            using (var entities = new gb_dvsstagingEntities())
            {
                var val_product = entities.dvs_product.Find(id);

                return new Product()
                {
                    Id = val_product.Id,
                    Name = val_product.Name
                };
            }
        }

        public LastArrivalMessagesInfo LastArrivalMessagesInfo(int? productId)
        {
            using (var entities = new gb_dvsstagingEntities())
            {
                var info = new LastArrivalMessagesInfo();

                var lastArrivalMessage = entities.dvs_arrivalmessage
                    .Where(x => (productId == null || x.ProductId == productId))
                    .OrderByDescending(x=>x.Id)
                    .FirstOrDefault();

                if (lastArrivalMessage != null)
                {
                    info.LastDownloadedAt = lastArrivalMessage.CreatedOn;
                }

                var date = DateTime.UtcNow;
                date = date.AddHours(-1);

                var count = entities.dvs_arrivalmessage
                    .Count(x => x.CreatedOn > date && (productId == null || x.ProductId == productId));

                info.CountLastHour = count;

                return info;
            }
        }
    }
}
