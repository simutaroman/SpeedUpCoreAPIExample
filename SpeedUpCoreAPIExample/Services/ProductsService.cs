using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpeedUpCoreAPIExample.Exceptions;
using SpeedUpCoreAPIExample.Interfaces;
using SpeedUpCoreAPIExample.Models;
using SpeedUpCoreAPIExample.Settings;
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
        private readonly IPricesCacheRepository _pricesCacheRepository;
        private readonly IProductCacheRepository _productCacheRepository;

        public ProductsService(IProductsRepository productsRepository, IPricesCacheRepository pricesCacheRepository,
            IProductCacheRepository productCacheRepository, IOptions<ProductsSettings> settings, ISelfHttpClient selfHttpClient)
        {
            _productsRepository = productsRepository;
            _selfHttpClient = selfHttpClient;
            _pricesCacheRepository = pricesCacheRepository;
            _productCacheRepository = productCacheRepository;
        }

        public async Task<IEnumerable<ProductViewModel>> FindProductsAsync(string sku)
        {
            IEnumerable<Product> products = await _productsRepository.FindProductsAsync(sku);

            if (products.Count() == 1)
            {
                //only one record found
                Product product = products.FirstOrDefault();
                string productId = product.ProductId.ToString();

                //cache a product if not in cache yet
                if (!await _productCacheRepository.IsValueCachedAsync(productId))
                {
                    await _productCacheRepository.SetValueAsync(productId, product);
                }

                //prepare prices
                if (!await _pricesCacheRepository.IsValueCachedAsync(productId))
                {
                    //prepare prices beforehand
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        CallPreparePricesApiAsync(productId);
                    });
                }
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
            Product product = await _productCacheRepository.GetOrSetValueAsync(productId.ToString(), async () => await _productsRepository.GetProductAsync(productId));

            if (product == null)
                throw new HttpException(HttpStatusCode.NotFound, "Product not found", $"Product Id: {productId}");

            //prepare prices
            if (!await _pricesCacheRepository.IsValueCachedAsync(productId.ToString()))
            {
                //prepare prices beforehand
                ThreadPool.QueueUserWorkItem(delegate
                {
                    CallPreparePricesApiAsync(productId.ToString());
                });
            }

            return new ProductViewModel(product);
        }

        public async Task<ProductViewModel> DeleteProductAsync(int productId)
        {
            Product product = await _productsRepository.DeleteProductAsync(productId);

            if (product == null)
                throw new HttpException(HttpStatusCode.NotFound, "Product not found", $"Product Id: {productId}");

            //remove product and its prices from cache
            await _productCacheRepository.RemoveValueAsync(productId.ToString());
            await _pricesCacheRepository.RemoveValueAsync(productId.ToString());

            return new ProductViewModel(product);
        }

        private async void CallPreparePricesApiAsync(string productId)
        {
            await _selfHttpClient.PostIdAsync("prices/prepare", productId);
        }
    }
}