using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeedUpCoreAPIExample.Exceptions;
using SpeedUpCoreAPIExample.Interfaces;
using SpeedUpCoreAPIExample.Models;
using SpeedUpCoreAPIExample.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly ISelfHttpClient _selfHttpClient;

        public ProductsService(IProductsRepository productsRepository, ISelfHttpClient selfHttpClient)
        {
            _productsRepository = productsRepository;
            _selfHttpClient = selfHttpClient;
        }

        public async Task<IEnumerable<ProductViewModel>> FindProductsAsync(string sku)
        {
            IEnumerable<Product> products = await _productsRepository.FindProductsAsync(sku);

            if (products.Count() == 1)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    CallPreparePricesApiAsync(products.FirstOrDefault().ProductId);
                });
            };
            return products.Select(p => new ProductViewModel(p));
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllProductsAsync()
        {
            IEnumerable<Product> products = await _productsRepository.GetAllProductsAsync();

            return products.Select(p => new ProductViewModel(p));
        }

        public async Task<ProductViewModel> GetProductAsync(int productId)
        {
            Product product = await _productsRepository.GetProductAsync(productId);

            if (product == null)
                throw new HttpException(HttpStatusCode.NotFound, "Product not found", $"Product Id: {productId}");

            ThreadPool.QueueUserWorkItem(delegate
            {
                CallPreparePricesApiAsync(productId);
            });

            return new ProductViewModel(product);
        }

        public async Task<ProductViewModel> DeleteProductAsync(int productId)
        {
            Product product = await _productsRepository.DeleteProductAsync(productId);

            if (product == null)
                throw new HttpException(HttpStatusCode.NotFound, "Product not found", $"Product Id: {productId}");

            return new ProductViewModel(product);
        }

        private async void CallPreparePricesApiAsync(int productId)
        {
            await _selfHttpClient.PostIdAsync("prices/prepare", productId.ToString());
        }


    }
}