using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSports.Framework.Models.CustomClasses;
using LSports.Services.ViewModels;

namespace LSports.Services.Interfaces
{
    public interface IProductService
    {
        IList<ProductViewModel> GetProductsViewModel();
        IList<Product> GetAll();

        Dictionary<string, int> GetHistoricalData(int productId, DateTime from, DateTime to);

        Product GetProduct(int id);
        LastArrivalMessagesInfo LastArrivalMessagesInfo(int? id); 
    }
}
