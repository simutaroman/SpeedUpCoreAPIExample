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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public ProductsService(IProductsRepository productsRepository, IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory)
        {
            _productsRepository = productsRepository;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _apiUrl = GetFullyQualifiedApiUrl("/api/prices/prepare/");
        }

        public async Task<IEnumerable<ProductViewModel>> FindProductsAsync(string sku)
        {
            IEnumerable<Product> products = await _productsRepository.FindProductsAsync(sku);

            if (products.Count() == 1)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    PreparePricesAsync(products.FirstOrDefault().ProductId);
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
                PreparePricesAsync(productId);
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

        private async void PreparePricesAsync(int productId)
        {
            var parameters = new Dictionary<string, string>();
            var encodedContent = new FormUrlEncodedContent(parameters);

            HttpClient client = _httpClientFactory.CreateClient();
            var result = await client.PostAsync(_apiUrl + productId, encodedContent).ConfigureAwait(false);
        }

        private string GetFullyQualifiedApiUrl(string apiRout)
        {
            string apiUrl = string.Format("{0}://{1}{2}",
                            _httpContextAccessor.HttpContext.Request.Scheme,
                            _httpContextAccessor.HttpContext.Request.Host,
                            apiRout);

            return apiUrl;
        }

    }
}