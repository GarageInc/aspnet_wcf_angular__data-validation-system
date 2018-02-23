using System;
using System.Collections.Generic;
using System.Linq;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.DataAccess.Repositories.Interfaces;
using LSports.Services.Interfaces;
using LSports.Services.ViewModels;
using LSports.Framework.Models.CustomClasses;

namespace LSports.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidationResultRepository _validationResultRepository;

        public ProductService() : this(new ProductRepository(), new ValidationResultRepository())
        {
        }

        public ProductService(IProductRepository productRepository, IValidationResultRepository validationResultRepository)
        {
            _productRepository = productRepository;
            _validationResultRepository = validationResultRepository;
        }

        public IList<ProductViewModel> GetProductsViewModel()
        {
            var products = _productRepository.GetList();

            var result =
                products.Select(
                    p =>
                        new ProductViewModel
                        {
                            Product = p,
                            NumberOfIssues = _validationResultRepository.GetNumberOfResultsByProductId(p.Id)
                        }).ToList();

            return result;
        }


        public IList<Product> GetAll()
        {
            return _productRepository.GetList();
        }

        public Dictionary<string, int> GetHistoricalData(int productId, DateTime from, DateTime to)
        {
            return _validationResultRepository.GetHistoricalDataByProductIdAndTimeFrame(productId, from, to);
        }

        public Product GetProduct(int id)
        {
            return _productRepository.GetProduct(id);
        }

        public LastArrivalMessagesInfo LastArrivalMessagesInfo(int? id)
        {
            return _productRepository.LastArrivalMessagesInfo(id);
        }
    }
}