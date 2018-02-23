using System.Collections.Generic;
using LSports.Framework.Models.CustomClasses;

namespace LSports.DVS.Framework.DataAccess.Repositories.Interfaces
{
    public interface IProductRepository
    {
        IList<Product> GetList();

        IList<Product> GetListByType(bool IsPushService);

        Product GetProduct(int id);

        LastArrivalMessagesInfo LastArrivalMessagesInfo(int? id);
    }
}
